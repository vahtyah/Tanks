using UnityEngine;

public class CharacterMachineGun : CharacterAbility
{
    private Transform weaponHolder;
    private Transform projectileSpawnPoint;
    [SerializeField] private Weapon weapon;
    
    protected override void PreInitialize()
    {
        base.PreInitialize();
        weaponHolder = Character.Model.SkillWeaponHolder;
        projectileSpawnPoint = Character.Model.SkillSpawnPoint;
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