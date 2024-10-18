using System;
using MoreMountains.Feedbacks;
using Photon.Pun;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private MMF_Player rpcWeaponUseFeedback;
    [SerializeField] private MMF_Player localWeaponUseFeedback;
    [SerializeField] private float reloadTime = 1;
    public Character Owner { get; set; }

    private PhotonView photonView;
    private Transform projectileSpawnTransform;
    private Timer reloadTimer;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        reloadTimer = Timer.Register(reloadTime).AlreadyDone().AutoDestroyWhenOwnerDisappear(this);
    }

    private void Start() { Initialization(); }

    private void Initialization() { }

    public void WeaponUse()
    {
        if (!reloadTimer.IsCompleted)
            return;
        reloadTimer.Reset();
        photonView.RPC(nameof(WeaponUseRPC), RpcTarget.All);
        localWeaponUseFeedback?.PlayFeedbacks();
    }

    [PunRPC]
    void WeaponUseRPC(PhotonMessageInfo info)
    {
        var nextProjectile = Pool.Spawn(projectilePrefab, projectileSpawnTransform.position,
            projectileSpawnTransform.rotation);
        var projectile = nextProjectile.GetComponent<Projectile>();
        if (projectile != null)
        {
            var lag = (float)(PhotonNetwork.Time - info.SentServerTime);
            projectile.SetOwner(Owner.gameObject)
                .SetWeapon(this)
                .SetLag(Mathf.Abs(lag))
                .OnSpawn();
        }
        rpcWeaponUseFeedback?.PlayFeedbacks();
    }

    public void SetProjectileSpawnTransform(Transform projectileSpawnPoint)
    {
        projectileSpawnTransform = projectileSpawnPoint;
    }

    public void AddOnReloadListener(Action<float> updateBar) { reloadTimer.OnProgress(updateBar); }
}