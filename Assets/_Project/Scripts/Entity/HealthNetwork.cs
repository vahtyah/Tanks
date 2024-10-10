using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthNetwork : NetworkBehaviour
{
    public float maxHealth = 20;
    public NetworkVariable<float> health = new(20);
    public GameObject tankExplosionEffect;
    
    
    public Action<float> OnHit;

    public bool TryKill(float damage)
    {
        // if (!IsServer)
        // {
        //     Debug.LogWarning("Only the server can apply damage to the health");
        //     return false;
        // }

        health.Value -= damage;
        // OnHit?.Invoke(GetHealthAmountNormalized());
        InvokeOnHitClientRpc(GetHealthAmountNormalized());
        
        if (health.Value <= 0)
        {
            OnDeath();
            return true;
        }

        return false;
    }

    private float GetHealthAmountNormalized()
    {
        return health.Value / maxHealth;
    }

    public bool IsAlive() { return health.Value >= 0; }

    private void OnDeath()
    {
        LevelManagerNetCode.Instance.RemovePlayer(OwnerClientId);
        NotifyDeathClientRpc();
        NetworkObjectDespawner.Despawn(NetworkObject);
        NetworkObjectSpawner.Spawn(tankExplosionEffect, transform.position);
    }
    


    [ClientRpc]
    private void NotifyDeathClientRpc(ClientRpcParams rpcParams = default)
    {
        // Kiểm tra nếu client là owner
        if (IsOwner)
        {
            GUIManagerNetcode.Instance.SetDiePanel(true);
            LevelManagerNetCode.Instance.isGameOver = true;
        }
    }
    
    [ClientRpc]
    private void InvokeOnHitClientRpc(float healthAmountNormalized, ClientRpcParams rpcParams = default)
    {
        OnHit?.Invoke(healthAmountNormalized);
    }

    public void AddOnHitListener(Action<float> updateBar)
    {
        OnHit += updateBar;
    }
}