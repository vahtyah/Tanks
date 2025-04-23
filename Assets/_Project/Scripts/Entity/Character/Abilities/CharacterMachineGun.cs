using Photon.Pun;
using UnityEngine;

public class CharacterMachineGun : CharacterAbility
{
    private Transform weaponHolder;
    private Transform projectileSpawnPoint;
    [SerializeField] private Weapon weapon;
    [SerializeField] private float multiplierSpeed = 0.5f;
    [SerializeField] private Sprite icon;
    

    private CharacterMovement movement;
    private float multiplierSpeedStorage;
    // private StateMachine<WeaponState> weaponState;

    protected override void PreInitialize()
    {
        base.PreInitialize();
        if (PhotonView.IsMine)
        {

            // weaponState = new StateMachine<WeaponState>(gameObject, TriggerType.Once);
            // weaponState.ChangeState(WeaponState.Initializing, weapon.WeaponCountdownDuration);
            //
            // weapon.weaponCooldownTimer.OnStart(()=> weaponState.ChangeState(WeaponState.Firing, weapon.RemainingProjectiles));
            // weapon.magazineReloadTimer.OnStart(()=> weaponState.ChangeState(WeaponState.Reloading, weapon.MagazineReloadDuration))
            //     .OnTimeRemaining((remaining)=> weaponState.ChangeState(WeaponState.Reloading, remaining));

            SkillEvent.TriggerEvent(WeaponState.Initializing, weapon.MagazineReloadDuration, icon);

            weapon.weaponCooldownTimer.OnStart(() =>
                SkillEvent.TriggerEvent(WeaponState.Firing, weapon.RemainingProjectiles));
            weapon.magazineReloadTimer.OnStart(() =>
                    SkillEvent.TriggerEvent(WeaponState.Reloading, weapon.MagazineReloadDuration))
                .OnComplete(() => SkillEvent.TriggerEvent(WeaponState.Ready, weapon.RemainingProjectiles));

            weaponHolder = Character.Model.SkillWeaponHolder;
            movement = GetComponent<CharacterMovement>();
        }
        projectileSpawnPoint = Character.Model.SkillSpawnPoint;
        EquipWeapon();
    }
    
    [PunRPC]
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