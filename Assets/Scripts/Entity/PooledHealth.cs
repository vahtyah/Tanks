using UnityEngine;

public class PooledHealth : Health
{
    protected override void OnDeath()
    {
        Pool.Return(gameObject);
    }
}
