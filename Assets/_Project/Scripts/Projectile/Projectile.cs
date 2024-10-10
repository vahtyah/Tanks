using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour, IPoolable
{
    //Setting
    public float speed = 20f;
    public float lifeTime = 1f;
    public float Acceleration = 0;
    private DamageOnTouch damageOnTouch;
    public GameObject owner;
    private NetworkObject networkObject;

    //Movement
    private Rigidbody rb;
    Vector3 moveDirection;
    private float currentSpeed;

    //Pooling
    private Health health;

    WeaponNetwork weaponNetwork;

    // Timer timer;
    
    private float timer = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        damageOnTouch = GetComponent<DamageOnTouch>();
        health = GetComponent<Health>();
        networkObject = GetComponent<NetworkObject>();
    }

    // private void OnEnable() { OnSpawn(); }

    private void Start()
    {
        // timer = Timer.Register(lifeTime)
        //     .OnComplete(DespawnProjectile)
        //     .AutoDestroyWhenOwnerDisappear(this)
        //     .Start();
    }

    public void DespawnProjectile()
    {
        health.Reset();
        damageOnTouch.ClearIgnoreObjects();
        if(networkObject == null || !networkObject.IsSpawned)
        {
            Pool.Return(gameObject);
            return;
        }
        DespawnGameObjectServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnGameObjectServerRpc()
    {
        
        networkObject.Despawn(true);
        // Pool.Return(gameObject);
    }

    private void OnDisable() { OnDespawn(); }

    private void FixedUpdate()
    {
        Movement(); 
        
        timer += Time.fixedDeltaTime;
        if (timer >= lifeTime)
        {
            DespawnProjectile();
        }
    }

    //Chuyển sang sử dụng Abilities
    void Movement()
    {
        moveDirection = transform.forward * ((currentSpeed / 10) * Time.fixedDeltaTime);
        rb.MovePosition(transform.position + moveDirection);
        currentSpeed += Acceleration * Time.fixedDeltaTime;
    }

    public void OnSpawn()
    {
        // timer?.Reset();
        timer = 0;
        damageOnTouch.AddIgnoreObject(owner);
        currentSpeed = speed;
    }

    public void OnDespawn() { }

    public void SetOwner(GameObject o)
    {
        owner = o;
    }

    public void SetWeapon(WeaponNetwork weaponNetwork) { this.weaponNetwork = weaponNetwork; }
}