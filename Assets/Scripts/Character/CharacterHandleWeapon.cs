using UnityEngine;

public class CharacterHandleWeapon : CharacterAbility
{
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectilePrefab;
    
    [SerializeField] private float fireRate = 1f;

    private Timer fireTimer;

    protected override void Initialization()
    {
        base.Initialization();
        Pool.Register(projectilePrefab);
        fireTimer = Timer.Register(fireRate).AlreadyDone();
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        HandleInput();
    }

    protected override void HandleInput()
    {
        base.HandleInput(); 
        if (characterInput.GetFireButton() && fireTimer.IsCompleted)
        {
            Fire();
            fireTimer.Restart();
        }
    }

    private void Fire()
    {
        var projectile = Pool.Get(projectilePrefab);
        projectile.transform.position = projectileSpawnPoint.position;
        projectile.transform.rotation = projectileSpawnPoint.rotation;
    }
}