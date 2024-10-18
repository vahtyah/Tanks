using System;
using ExitGames.Client.Photon;
using MoreMountains.Feedbacks;
using Photon.Pun;
using UnityEngine;

public class CharacterHealth : Health, IEventListener<GameEvent>
{
    public GameObject tankExplosionEffect;
    [SerializeField] private ForceFieldController protectionShield;
    [SerializeField] private int timeInvulnerableAfterSpawn = 3;
    private Character character;
    private Timer invulnerableTimer;

    protected override void Awake()
    {
        base.Awake();
        character = GetComponent<Character>();
        invulnerableTimer = Timer.Register(timeInvulnerableAfterSpawn)
            .OnStart(() => { photonView.RPC(nameof(SetInvulnerable), RpcTarget.All, true); })
            .OnComplete(() => { photonView.RPC(nameof(SetInvulnerable), RpcTarget.All, false); })
            .AutoDestroyWhenOwnerDisappear(this);
        //TODO: Debug
        timeInvulnerableAfterSpawn = GameManager.Instance.invulnerableTime == 0 ? timeInvulnerableAfterSpawn : GameManager.Instance.invulnerableTime;
    }

    [PunRPC]
    private void SetInvulnerable(bool b)
    {
        Invulnerable = b;
        if (b)
        {
            protectionShield.gameObject.SetActive(true);
        }
        else
        {
            protectionShield.SetInvulnerable(b);
        }
    }

    protected override void OnDeath(int lastHitBy)
    {
        photonView.RPC(nameof(OnDeathRPC), RpcTarget.All);
        if (!photonView.IsMine) return;
        photonView.Owner.SetCustomProperties(new Hashtable()
        {
            { GlobalString.PLAYER_DIED, true },
            { GlobalString.PLAYER_DEATHS, (int)photonView.Owner.CustomProperties[GlobalString.PLAYER_DEATHS] + 1 }
        });

        PhotonNetwork.GetPhotonView(lastHitBy).Owner.SetCustomProperties(new Hashtable()
        {
            {
                GlobalString.PLAYER_KILLS,
                (int)PhotonNetwork.GetPhotonView(lastHitBy).Owner.CustomProperties[GlobalString.PLAYER_KILLS] + 1
            }
        });

        PhotonNetwork.Destroy(photonView);
        // Pool.PhotonDespawn(photonView);
    }

    [PunRPC]
    private void OnDeathRPC()
    {
        character.conditionState.ChangeState(CharacterStates.CharacterCondition.Dead);
        CharacterEvent.Trigger(CharacterEventType.CharacterDeath, GetComponent<Character>());
        var effect = Pool.Spawn(tankExplosionEffect, null).GetComponent<ParticleSystem>();
        effect.transform.position = transform.position;
        effect.Play();
    }

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GameRunning:
                invulnerableTimer.Start();
                break;
        }
    }

    private void OnEnable() { this.StartListening(); }

    private void OnDisable() { this.StopListening(); }
}