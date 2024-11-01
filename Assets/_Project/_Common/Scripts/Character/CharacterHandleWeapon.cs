using MoreMountains.Feedbacks;
using Photon.Pun;
using UnityEngine;

public class CharacterHandleWeapon : CharacterAbility
{
    //Binding
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
        // weapon = Instantiate(weapon, weaponHolder);
        Character.EquipWeapon(weapon);
        weapon.Owner = Character;
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
        if (Controller.GetFire() && GameManager.Instance.currentGameType == GameEventType.GameRunning)
        {
            weapon.UseWeapon();
            // if (weapon.WeaponUse())
            //     weaponUseFeedback?.PlayFeedbacks();
        }
    }
}