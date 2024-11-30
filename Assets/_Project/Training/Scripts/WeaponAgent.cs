using Unity.MLAgents;
using UnityEngine;

public class WeaponAgent : Weapon
{
    private CharacterAgent agent;
    public override void SetOwner(CharacterAgent agent)
    {
        this.agent = agent;
    }

    public override bool UseWeapon()
    {
        if (!weaponCooldownTimer.IsCompleted || magazineReloadTimer.IsRunning)
            return false;
        weaponCooldownTimer.ReStart();
        var newProjectile = Pool.Spawn(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        if (newProjectile.TryGetComponent(out Projectile projectile))
        {
            projectile.SetOwner(agent)
                .SetWeapon(this)
                .OnSpawn();
        }
        
        globalWeaponUseFeedback?.PlayFeedbacks();
        localWeaponUseFeedback?.PlayFeedbacks();
        return true;
    }
}
