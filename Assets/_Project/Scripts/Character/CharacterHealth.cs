using Unity.Netcode;
using UnityEngine;

public class CharacterHealth : Health
{
    public GameObject tankExplosionEffect;

    protected override void Start()
    {
        base.Start();
        Pool.Register(tankExplosionEffect);
    }

    protected override void OnDeath()
    {
        character.conditionState.ChangeState(CharacterStates.CharacterCondition.Dead);
        CharacterEvent.Trigger(CharacterEventType.CharacterDeath, GetComponent<Character>());
        if (NetworkManager != null)
        {
            TankExplosionServerRpc();
            GetComponent<NetworkObject>().Despawn();
        }
        else
        {
            var part= Pool.Spawn(tankExplosionEffect, true);
            part.transform.position = transform.position;
            gameObject.SetActive(false);
        }
    }
    
    [ServerRpc]
    private void TankExplosionServerRpc()
    {
        NetworkObjectSpawner.Spawn(tankExplosionEffect, transform.position, Quaternion.identity);
    }
}
