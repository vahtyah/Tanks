using MoreMountains.Feedbacks;
using Unity.Netcode;
using UnityEngine;

public class CharacterHandleWeapon : CharacterAbility
{
    //Binding
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private MMF_Player weaponUseFeedback;
    [SerializeField] private MMF_Player weaponUseFeedbackOwer;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private WeaponNetwork weaponNetwork;

    protected override void Initialization()
    {
        Pool.Register(projectilePrefab);
        InitWeapon();
        base.Initialization();
        //NOTE: Clear
    }
    private void InitWeapon()
    {
        weaponNetwork.Owner = character;
        weaponNetwork.SetProjectileSpawnTransform(projectileSpawnPoint);
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
            if (weaponNetwork.WeaponUse())
            {
                if (NetworkManager != null)
                {
                    PlayParticleServerRpc();
                    weaponUseFeedbackOwer?.PlayFeedbacks();
                }
                else
                {
                    weaponUseFeedback?.PlayFeedbacks();
                }
            }
        }
    }
    
    [ServerRpc] //yêu cầu server kích hoạt particle system
    private void PlayParticleServerRpc()
    {
        PlayParticleClientRpc();
    }
    
    [ClientRpc] //ClientRpc để kích hoạt particle trên tất cả các client
    private void PlayParticleClientRpc()
    {
       weaponUseFeedback?.PlayFeedbacks();
    }
}