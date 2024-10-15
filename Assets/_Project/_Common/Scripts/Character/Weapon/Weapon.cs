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

    private void Awake() { photonView = GetComponent<PhotonView>(); }

    private void Start() { Initialization(); }

    private void Initialization()
    {
        reloadTimer = Timer.Register(reloadTime).AlreadyDone();
    }

    [PunRPC]
    void WeaponUse(PhotonMessageInfo info)
    {
        if (!reloadTimer.IsCompleted)
            return;
        reloadTimer.Reset();
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

        if (photonView.IsMine)
            localWeaponUseFeedback?.PlayFeedbacks();
        rpcWeaponUseFeedback?.PlayFeedbacks();
    }

    public void SetProjectileSpawnTransform(Transform projectileSpawnPoint)
    {
        projectileSpawnTransform = projectileSpawnPoint;
    }
}