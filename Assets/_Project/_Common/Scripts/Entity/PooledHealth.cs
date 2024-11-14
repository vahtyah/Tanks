using UnityEngine;

public class PooledHealth : HealthTest
{
    public override void TakeDamage(float damage, Character lastHitBy)
    {
        CurrentHealth -= damage;
    }

    protected override void DestroyEntity()
    {
        Pool.Despawn(gameObject);
    }

    public override void OnDeath()
    {
    }
}