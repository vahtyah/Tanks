using UnityEngine;

public class CharacterHandleWeapon : CharacterAbility
{
    //Binding
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectilePrefab;
    
    [SerializeField] private Weapon weapon;

    protected override void Initialization()
    {
        Pool.Register(projectilePrefab);
        weapon = Instantiate(weapon, weaponHolder);
        weapon.Owner = character;
        weapon.SetProjectileSpawnTransform(projectileSpawnPoint);
        base.Initialization();
        //NOTE: Clear
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        HandleInput();
    }

    protected override void HandleInput()
    {
        base.HandleInput(); 
        if (characterInput.GetFireButton())
        {
            weapon.WeaponUse();
        }
    }
}