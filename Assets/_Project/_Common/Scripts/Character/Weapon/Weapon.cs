using System;
using MoreMountains.Feedbacks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private MMF_Player globalWeaponUseFeedback;
    [SerializeField] private MMF_Player localWeaponUseFeedback;
    [SerializeField] private float reloadDuration = 1;
    public Character Owner { get; set; }

    private PhotonView photonView;
    private Transform projectileSpawnPoint;
    private Timer reloadTimer;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        reloadTimer = Timer.Register(reloadDuration)
            .AlreadyDone()
            .AutoDestroyWhenOwnerDisappear(this);
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
    }

    public void UseWeapon()
    {
        if (!reloadTimer.IsCompleted)
            return;
        reloadTimer.Reset();
        photonView.RPC(nameof(UseWeaponRPC), RpcTarget.All);
        localWeaponUseFeedback?.PlayFeedbacks();
    }

    [PunRPC]
    private void UseWeaponRPC(PhotonMessageInfo info)
    {
        var spawnedProjectile = Pool.Spawn(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        if (spawnedProjectile.TryGetComponent(out Projectile projectile))
        {
            float networkLag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            projectile.SetOwner(Owner.gameObject)
                .SetWeapon(this)
                .SetLag(networkLag)
                .OnSpawn();
        }

        globalWeaponUseFeedback?.PlayFeedbacks();
    }

    public void SetProjectileSpawnPoint(Transform spawnPoint)
    {
        this.projectileSpawnPoint = spawnPoint;
    }

    public void RegisterOnReloadListener(Action<float> updateReloadUI)
    {
        reloadTimer.OnProgress(updateReloadUI);
    }
}