public class WeaponAgentNavMesh : Weapon
{
    private AgentNavMesh agent;
    public void SetOwner(AgentNavMesh agent)
    {
        this.agent = agent;
    }
    
    public override bool UseWeapon()
    {
        if (!weaponCooldownTimer.IsCompleted || magazineReloadTimer.IsRunning)
            return false;
        weaponCooldownTimer.ReStart();
        var newProjectile = Pool.Spawn(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        if (newProjectile.TryGetComponent(out ProjectileNavMesh projectile))
        {
            projectile.SetOwner1(agent)
                .SetWeapon(this)
                .OnSpawn();
        }
        
        globalWeaponUseFeedback?.PlayFeedbacks();
        localWeaponUseFeedback?.PlayFeedbacks();
        return true;
    }
}