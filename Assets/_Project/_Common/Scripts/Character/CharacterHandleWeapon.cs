using MoreMountains.Feedbacks;
using Photon.Pun;
using UnityEngine;

public class CharacterHandleWeapon : CharacterAbility
{
    //Binding
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Weapon weapon;
    [SerializeField] private PhotonView view;

    protected override void Initialization()
    {
        // weapon = Instantiate(weapon, weaponHolder);
        weapon.Owner = character;
        weapon.SetProjectileSpawnTransform(projectileSpawnPoint);
        base.Initialization();
        view = weapon.GetComponent<PhotonView>();
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
        if (controller.GetFire() && GameManager.Instance.currentGameType == GameEventType.GameStart)
        {
            view.RPC("WeaponUse", RpcTarget.All);
            // if (weapon.WeaponUse())
            //     weaponUseFeedback?.PlayFeedbacks();
        }
    }
}