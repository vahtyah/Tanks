using System;
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
    public float WeaponCountdownDuration => weaponCountdownDuration;

    [FoldoutGroup("Magazine")] [SerializeField]
    private bool useMagazine = false;

    [FoldoutGroup("Magazine"), ShowIf(nameof(useMagazine))] [SerializeField]
    private int magazineCapacity = 30;

    [FoldoutGroup("Magazine"), ShowIf(nameof(useMagazine))] [SerializeField]
    private bool autoReload = true;

    [FoldoutGroup("Magazine"), ShowIf(nameof(useMagazine))] [SerializeField]
    private float magazineReloadDuration = 1;

    public float MagazineReloadDuration => magazineReloadDuration;

    private int remainingProjectiles;

    public int RemainingProjectiles => remainingProjectiles;

    public Character WeaponOwner { get; set; }

    private PhotonView photonView;
    private Transform projectileSpawnPoint;
    public Timer weaponCooldownTimer { get; private set; }
    public Timer magazineReloadTimer { get; private set; }
    private float currentReloadTime;


    private void Awake()
    {
        // stateMachine = new WeaponStateMachine(weaponCountdownDuration);
        // stateMachine.ChangeState(WeaponState.Initializing);

        photonView = GetComponent<PhotonView>();
        remainingProjectiles = magazineCapacity;
        weaponCooldownTimer = Timer.Register(new LocalTimer(weaponCountdownDuration))
            .Ready()
            .AutoDestroyWhenOwnerDisappear(this);

        magazineReloadTimer = Timer.Register(new LocalTimer(magazineReloadDuration))
            .OnComplete(() => { remainingProjectiles = magazineCapacity; })
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

    public bool UseWeapon()
    {
        if (!weaponCooldownTimer.IsCompleted || magazineReloadTimer.IsRunning)
            return false;
        weaponCooldownTimer.ReStart();
        photonView.RPC(nameof(UseWeaponRPC), RpcTarget.All);
        localWeaponUseFeedback?.PlayFeedbacks();
        return true;
    }

    public bool IsMagazineEmpty()
    {
        return remainingProjectiles <= 0;
    }

    [PunRPC]
    private void UseWeaponRPC(PhotonMessageInfo info)
    {
        var spawnedProjectile =
            Pool.Spawn(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        if (spawnedProjectile.TryGetComponent(out Projectile projectile))
        {
            float networkLag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            projectile.SetOwner(WeaponOwner)
                .SetWeapon(this)
                .SetLag(networkLag)
                .OnSpawn();
        }

        globalWeaponUseFeedback?.PlayFeedbacks();

        if (useMagazine)
        {
            remainingProjectiles--;
            if (remainingProjectiles <= 0)
            {
                magazineReloadTimer.ReStart();
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