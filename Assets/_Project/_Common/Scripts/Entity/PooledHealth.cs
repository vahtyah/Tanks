using UnityEngine;

public class PooledHealth : Health
{
    protected override void OnDeath(int lastHitBy)
    {
        Pool.Despawn(gameObject);
    }
}
