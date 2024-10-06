using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable
{
    //Setting
    public float speed = 20f;
    public float lifeTime = 1f;
    public float Acceleration = 0;
    private DamageOnTouch damageOnTouch;
    private GameObject owner;

    //Movement
    private Rigidbody rb;
    Vector3 moveDirection;
    private float currentSpeed;

    //Pooling
    private Health health;
    
    Weapon weapon;

    Timer timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
        damageOnTouch = GetComponent<DamageOnTouch>();
        health = GetComponent<Health>();
    }

    private void OnEnable() { OnSpawn(); }

    private void Start()
    {
        timer = Timer.Register(lifeTime)
            .OnComplete(() => Pool.Return(gameObject))
            .AutoDestroyWhenOwnerDisappear(this)
            .Start();
    }

    private void OnDisable() { OnDespawn(); }

    private void FixedUpdate() { Movement(); }

    //Chuyển sang sử dụng Abilities
    void Movement()
    {
        moveDirection = transform.forward * ((currentSpeed / 10) * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + moveDirection);
        currentSpeed += Acceleration * Time.fixedDeltaTime;
    }

    public void OnSpawn()
    {
        timer?.Reset(); 
        damageOnTouch.AddIgnoreObject(owner);
        currentSpeed = speed;
    }

    public void OnDespawn()
    {
        health.Reset(); 
        damageOnTouch.ClearIgnoreObjects();
    }

    public void SetOwner(GameObject o)
    {
        owner = o;
    }

    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
    }
}