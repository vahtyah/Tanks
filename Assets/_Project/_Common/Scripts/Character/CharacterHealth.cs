using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class CharacterHealth : Health, IEventListener<GameEvent>
{
    [SerializeField] private CharacterCaptureFlag characterCaptureFlag;
    
    public GameObject TankExplosionEffect;
    [SerializeField] private ForceFieldController protectionShield;
    [SerializeField] private int timeInvulnerableAfterSpawn = 3;
    private Character character;
    private Timer invulnerableTimer;

    protected override void Awake()
    {
        base.Awake();
        character = GetComponent<Character>();

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
        PhotonView.RPC(nameof(SetInvulnerable), RpcTarget.All, isInvulnerable);
    }

    [PunRPC]
    private void SetInvulnerable(bool isInvulnerable)
    {
        IsInvulnerable = isInvulnerable;

        if (!isInvulnerable)
        {
            protectionShield.SetInvulnerable(false);
        }
        else
        {
            protectionShield.gameObject.SetActive(true);
        }
    }

    protected override void OnDeath(int lastHitBy)
    {
        PhotonView.RPC(nameof(OnDeathRPC), RpcTarget.All);
        characterCaptureFlag.ReleaseCapturedFlag();

        if (!PhotonView.IsMine) return;

        UpdatePlayerStats(lastHitBy);
        PhotonNetwork.Destroy(PhotonView);
    }

    private void UpdatePlayerStats(int lastHitBy)
    {
        var currentPlayerProperties = new Hashtable
        {
            { GlobalString.PLAYER_DIED, true },
            { GlobalString.PLAYER_DEATHS, (int)PhotonView.Owner.CustomProperties[GlobalString.PLAYER_DEATHS] + 1 }
        };
        PhotonView.Owner.SetCustomProperties(currentPlayerProperties);

        var killerProperties = new Hashtable
        {
            {
                GlobalString.PLAYER_KILLS,
                (int)PhotonNetwork.GetPhotonView(lastHitBy).Owner.CustomProperties[GlobalString.PLAYER_KILLS] + 1
            }
        };
        PhotonNetwork.GetPhotonView(lastHitBy).Owner.SetCustomProperties(killerProperties);
    }

    [PunRPC]
    private void OnDeathRPC()
    {
        character.ConditionState.ChangeState(CharacterStates.CharacterCondition.Dead);
        CharacterEvent.Trigger(CharacterEventType.CharacterDeath, character);

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