using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float reloadTime = 1;
    public Character Owner { get; set; }
    
    private Transform projectileSpawnTransform;
    private Timer reloadTimer;

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        Pool.Register(projectilePrefab);
        reloadTimer = Timer.Register(reloadTime).AlreadyDone();
    }

    public bool WeaponUse()
    {
        if(!reloadTimer.IsCompleted)
            return false;
        reloadTimer.Reset();
        var nextProjectile = Pool.Get(projectilePrefab, false);
        nextProjectile.transform.position = projectileSpawnTransform.position;
        nextProjectile.transform.rotation = projectileSpawnTransform.rotation;
        var projectile = nextProjectile.GetComponent<Projectile>();
        if(projectile != null)
        {
            projectile.SetOwner(Owner.gameObject);
            projectile.SetWeapon(this);
        }
        
        nextProjectile.SetActive(true);
        return true;
    }

    public void SetProjectileSpawnTransform(Transform projectileSpawnPoint)
    {
        projectileSpawnTransform = projectileSpawnPoint;
    }
}