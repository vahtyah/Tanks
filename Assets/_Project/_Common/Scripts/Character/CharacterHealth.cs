using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class CharacterHealth : Health, IEventListener<GameEvent>
{
    private CharacterFlagCapture characterFlagCapture;

    public GameObject TankExplosionEffect;
    [SerializeField] private ForceFieldController protectionShield;
    [SerializeField] private int timeInvulnerableAfterSpawn = 3;
    protected Character Character;
    private Timer invulnerableTimer;

    protected override void Awake()
    {
        base.Awake();
        Character = GetComponent<Character>();
        characterFlagCapture = GetComponent<CharacterFlagCapture>();

        invulnerableTimer = Timer.Register(timeInvulnerableAfterSpawn)
            .OnStart(() => SetInvulnerableRPC(true))
            .OnComplete(() => SetInvulnerableRPC(false))
            .AutoDestroyWhenOwnerDisappear(this);

        timeInvulnerableAfterSpawn = GameManager.Instance.invulnerableTime == 0
            ? timeInvulnerableAfterSpawn
            : GameManager.Instance.invulnerableTime;
    }

    private void SetInvulnerableRPC(bool isInvulnerable)
    {
        if (PhotonView.IsMine)
            PhotonView.RPC(nameof(SetInvulnerable), RpcTarget.All, isInvulnerable);
    }

    [PunRPC]
    private void SetInvulnerable(bool isInvulnerable)
    {
        IsInvulnerable = isInvulnerable;
        protectionShield.HandleOpenClose(isInvulnerable);
    }

    protected override void OnDeath(int lastHitBy)
    {
        PhotonView.RPC(nameof(OnDeathRPC), RpcTarget.All);
        characterFlagCapture.ReleaseCapturedFlag(transform.position);

        if (!PhotonView.IsMine) return;

        UpdatePlayerStats(lastHitBy);
        PhotonNetwork.Destroy(PhotonView);
    }

    private void UpdatePlayerStats(int lastHitBy)
    {
        Character.SetPlayerDied(true);
        Character.AddDeath(1);
        Character.AddScore(-1);
        
        var killer = PhotonNetwork.GetPhotonView(lastHitBy).gameObject.GetComponent<Character>();
        killer.IncreaseMultiKillCount();
        killer.AddKill(1);
        killer.AddScore(killer.GetMultiKillScore());
    }

    [PunRPC]
    private void OnDeathRPC()
    {
        Character.ConditionState.ChangeState(CharacterStates.CharacterCondition.Dead);
        CharacterEvent.Trigger(CharacterEventType.CharacterDeath, Character);

        // Spawn death effect
        var effect = Pool.Spawn(TankExplosionEffect, null).GetComponent<ParticleSystem>();
        effect.transform.position = transform.position;
        effect.Play();
    }

    public void OnEvent(GameEvent e)
    {
        if (e.EventType == GameEventType.GameRunning)
        {
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