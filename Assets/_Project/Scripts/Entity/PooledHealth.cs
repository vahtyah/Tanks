using Unity.Netcode;
using UnityEngine;

public class PooledHealth : Health
{
    private Projectile projectile;

    protected override void Awake()
    {
        base.Awake();
        projectile = GetComponent<Projectile>();
    }

    protected override void OnDeath()
    {
        projectile.DespawnProjectile();
    }
}