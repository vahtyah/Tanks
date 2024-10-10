using System;
using Unity.Netcode;
using UnityEngine;

public class WeaponNetwork : NetworkBehaviour
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
        
        if(NetworkManager == null)
        {
            SpawnProjectile();
            return true;
        }
        
        SpawnProjectileServerRpc();
        return true;
    }
    [ServerRpc]
    private void SpawnProjectileServerRpc()
    {
        var nextProjectile = NetworkObjectSpawner.Spawn(projectilePrefab, projectileSpawnTransform.position, projectileSpawnTransform.rotation);
        var projectile = nextProjectile.GetComponent<Projectile>();
        if(projectile != null)
        {
            projectile.SetOwner(Owner.gameObject);
            projectile.SetWeapon(this);
            projectile.OnSpawn();
        }
    }

    private void SpawnProjectile()
    {
        var nextProjectile = Pool.Spawn(projectilePrefab, projectileSpawnTransform.position, projectileSpawnTransform.rotation);
        var projectile = nextProjectile.GetComponent<Projectile>();
        if(projectile != null)
        {
            projectile.SetOwner(Owner.gameObject);
            projectile.SetWeapon(this);
            projectile.OnSpawn();
        }
    }

    public void SetProjectileSpawnTransform(Transform projectileSpawnPoint)
    {
        projectileSpawnTransform = projectileSpawnPoint;
    }
}