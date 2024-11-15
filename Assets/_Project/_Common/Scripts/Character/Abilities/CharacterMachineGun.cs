using UnityEngine;

public class CharacterMachineGun : CharacterAbility
{
    private Transform weaponHolder;
    private Transform projectileSpawnPoint;
    [SerializeField] private Weapon weapon;
    [SerializeField] private float multiplierSpeed = 0.5f;


    private CharacterMovement movement;
    private float multiplierSpeedStorage;

    protected override void PreInitialize()
    {
        base.PreInitialize();
        weaponHolder = Character.Model.SkillWeaponHolder;
        projectileSpawnPoint = Character.Model.SkillSpawnPoint;
        movement = GetComponent<CharacterMovement>();
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

        if (Controller.GetInputSkillDown())
        {
            multiplierSpeedStorage = movement.SpeedMultiplier;
            movement.SpeedMultiplier = multiplierSpeed;
        }

        if (Controller.GetInputSkill())
        {
            if (weapon.UseWeapon())
            {
                movement.SpeedMultiplier = multiplierSpeed;
            }
            else if (weapon.IsMagazineEmpty())
            {
                movement.SpeedMultiplier = multiplierSpeedStorage;
            }
        }

        if (Controller.GetInputSkillUp())
        {
            movement.SpeedMultiplier = multiplierSpeedStorage;
        }
    }
}