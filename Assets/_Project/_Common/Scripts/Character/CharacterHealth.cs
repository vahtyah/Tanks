using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class CharacterHealth : Health
{
    public GameObject tankExplosionEffect;
    private Character character;

    protected override void Awake()
    {
        base.Awake();
        character = GetComponent<Character>();
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
            { GlobalString.PLAYER_KILLS, (int)PhotonNetwork.GetPhotonView(lastHitBy).Owner.CustomProperties[GlobalString.PLAYER_KILLS] + 1 }
        });
        
        PhotonNetwork.Destroy(photonView);
    }

    [PunRPC]
    private void OnDeathRPC()
    {
        character.conditionState.ChangeState(CharacterStates.CharacterCondition.Dead);
        CharacterEvent.Trigger(CharacterEventType.CharacterDeath, GetComponent<Character>());
        var effect = Pool.Spawn(tankExplosionEffect, transform.position).GetComponent<ParticleSystem>();
        effect.Play();
    }
}