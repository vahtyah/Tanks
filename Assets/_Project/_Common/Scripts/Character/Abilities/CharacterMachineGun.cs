using UnityEngine;

public class CharacterMachineGun : CharacterAbility
{
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Weapon weapon;
    
    protected override void PreInitialize()
    {
        base.PreInitialize();
        EquipWeapon();
    }
    
    private void EquipWeapon()
    {
        weapon.WeaponOwner = Character;
        weapon.SetProjectileSpawnPoint(projectileSpawnPoint);
    }
    
    public override void ProcessAbility()
    {
        base.ProcessAbility();
        HandleInput();
    }
    
    protected override void HandleInput()
    {
        base.HandleInput();
        if (Controller.GetInputSkill() )
        {
            weapon.UseWeapon();
        }
    }
    
}