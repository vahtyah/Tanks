using MoreMountains.Feedbacks;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChaHealth : HealthTest, IEventListener<GameEvent>
{
    [SerializeField, BoxGroup("Health Settings")]
    private ForceFieldController protectionShield;

    [SerializeField, BoxGroup("Health Settings")]
    private int timeInvulnerableAfterSpawn = 3;

    [SerializeField, BoxGroup("Feedbacks")]
    protected MMFeedbacks gotHitFeedbacks;

    private Character character;
    private Character lastHitBy;
    private Timer invulnerableTimer;
    private PhotonView photonView;

    protected override void Awake()
    {
        base.Awake();
        character = GetComponent<Character>();
        photonView = GetComponent<PhotonView>();

        invulnerableTimer = Timer.Register(timeInvulnerableAfterSpawn)
            .OnStart(() => SetInvulnerableRPC(true))
            .OnComplete(() => SetInvulnerableRPC(false))
            .AutoDestroyWhenOwnerDisappear(this);
    }

    private void SetInvulnerableRPC(bool isInvulnerable)
    {
        if (photonView.IsMine)
            photonView.RPC(nameof(SetInvulnerable), RpcTarget.All, isInvulnerable);
    }

    [PunRPC]
    private void SetInvulnerable(bool isInvulnerable)
    {
        IsInvulnerable = isInvulnerable;
        protectionShield.HandleOpenClose(isInvulnerable);
    }

    protected override void DestroyEntity()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(photonView);
        }
    }

    public override void TakeDamage(float damage, Character lastHitBy)
    {
        if (!photonView.IsMine) return;
        photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damage, lastHitBy.PhotonView.ViewID);
        gotHitFeedbacks?.PlayFeedbacks();
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, int lastHitByViewID)
    {
        lastHitBy = PhotonNetwork.GetPhotonView(lastHitByViewID).GetComponent<Character>();
        CurrentHealth -= damage;
    }


    public override void OnDeath()
    {
        // photonView.RPC(nameof(OnDeathRPC), RpcTarget.All);
        InGameEvent.Trigger(InGameEventType.SomeoneDied, lastHitBy, character);
        if (photonView.IsMine)
        {
            CharacterEvent.Trigger(CharacterEventType.CharacterDeath, character);
        }
    }

    [PunRPC]
    private void OnDeathRPC()
    {
        character.ConditionState.ChangeState(CharacterStates.CharacterCondition.Dead);
    }

    public override void ResetHealth()
    {
        Debug.Log(PhotonNetwork.OfflineMode);
        photonView.RPC(nameof(RPC_ResetHealth), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }

    public void OnEvent(GameEvent e)
    {
        if (e.EventType == GameEventType.GameRunning && photonView.IsMine)
        {
            ResetHealth();
            invulnerableTimer.Start();
        }
    }

    private void OnEnable()
    {
        this.StartListening();
    }

    private void OnDisable()
    {
        this.StopListening();
    }
}