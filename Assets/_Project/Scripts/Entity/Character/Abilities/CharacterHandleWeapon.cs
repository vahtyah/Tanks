using MoreMountains.Feedbacks;
using Photon.Pun;
using UnityEngine;

public class CharacterHandleWeapon : CharacterAbility
{
    //Binding
    private Transform weaponHolder;
    private Transform projectileSpawnPoint;
    [SerializeField] private Weapon weapon;

    protected override void PreInitialize()
    {
        base.PreInitialize();
        weaponHolder = Character.Model.PrimaryWeaponHolder;
        projectileSpawnPoint = Character.Model.PrimaryProjectileSpawnPoint;
        EquipWeapon();
    }

    private void EquipWeapon()
    {
        // weapon = Instantiate(weapon, weaponHolder);
        Character.EquipWeapon(weapon);
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
        if (Controller.GetFire() && GameManager.Instance.CurrentGameType == GameEventType.GameRunning)
        {
            weapon.UseWeapon();
            // if (weapon.WeaponUse())
            //     weaponUseFeedback?.PlayFeedbacks();
        }
    }
}