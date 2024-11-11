﻿using System;
using MoreMountains.Feedbacks;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private MMF_Player globalWeaponUseFeedback;
    [SerializeField] private MMF_Player localWeaponUseFeedback;
    [SerializeField] private float weaponCountdownDuration = 1;


    [FoldoutGroup("Magazine")] [SerializeField]
    private bool useMagazine = false;

    [FoldoutGroup("Magazine"), ShowIf(nameof(useMagazine))] [SerializeField]
    private int magazineCapacity = 30;

    [FoldoutGroup("Magazine"), ShowIf(nameof(useMagazine))] [SerializeField]
    private bool autoReload = true;

    [FoldoutGroup("Magazine"), ShowIf(nameof(useMagazine))] [SerializeField]
    private float magazineReloadDuration = 1;
    private int remainingProjectiles;

    public Character WeaponOwner { get; set; }

    private PhotonView photonView;
    private Transform projectileSpawnPoint;
    private Timer weaponCooldownTimer;
    private Timer magazineReloadTimer;

    private float currentReloadTime;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        remainingProjectiles = magazineCapacity;
        weaponCooldownTimer = Timer.Register(new LocalTimer(weaponCountdownDuration))
            .AlreadyDone()
            .AutoDestroyWhenOwnerDisappear(this);

        magazineReloadTimer = Timer.Register(new LocalTimer(magazineReloadDuration))
            .OnComplete(() =>
            {
                remainingProjectiles = magazineCapacity;
            })
            .AutoDestroyWhenOwnerDisappear(this);

        // reloadTimer = Timer.Register(reloadDuration)
        //     .AlreadyDone()
        //     .AutoDestroyWhenOwnerDisappear(this);
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
        if (!weaponCooldownTimer.IsCompleted || magazineReloadTimer.IsRunning)
            return;
        weaponCooldownTimer.Reset();
        photonView.RPC(nameof(UseWeaponRPC), RpcTarget.All);
        localWeaponUseFeedback?.PlayFeedbacks();
    }

    [PunRPC]
    private void UseWeaponRPC(PhotonMessageInfo info)
    {
        var spawnedProjectile =
            Pool.Spawn(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        if (spawnedProjectile.TryGetComponent(out Projectile projectile))
        {
            float networkLag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            projectile.SetOwner(WeaponOwner.gameObject)
                .SetWeapon(this)
                .OnSpawn();
        }

        globalWeaponUseFeedback?.PlayFeedbacks();

        if (useMagazine)
        {
            remainingProjectiles--;
            if (remainingProjectiles <= 0)
            {
                magazineReloadTimer.Reset();
            }
        }
    }

    public void SetProjectileSpawnPoint(Transform spawnPoint)
    {
        this.projectileSpawnPoint = spawnPoint;
    }

    public void RegisterOnReloadListener(Action<float> updateReloadUI)
    {
        weaponCooldownTimer.OnProgress(updateReloadUI);
    }
}