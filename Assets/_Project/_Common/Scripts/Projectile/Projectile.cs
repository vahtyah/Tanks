using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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
    
    Weapon weapon;

    Timer timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
        damageOnTouch = GetComponent<DamageOnTouch>();
    }

    private void Start()
    {
        timer = Timer.Register(lifeTime)
            .OnComplete(() => Pool.Despawn(gameObject))
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
        timer?.ReStart();
        damageOnTouch.AddToIgnoreList(owner);
        damageOnTouch.IgnoreTeamMembers();
        currentSpeed = speed;
    }

    public void OnDespawn()
    {
        damageOnTouch.ClearIgnoreList();
    }

    public Projectile SetOwner(GameObject o)
    {
        owner = o;
        damageOnTouch.SetOwner(owner.GetComponent<Character>());
        return this;
    }

    public Projectile SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
        return this;
    }
    
    public Projectile SetLag(float lag)
    {
        transform.position += rb.velocity * lag;
        return this;
    }
}