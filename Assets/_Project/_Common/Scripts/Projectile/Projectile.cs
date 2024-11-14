using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : Entity, IPoolable, IController
{
    //Setting
    [BoxGroup("Settings"),SerializeField] private float speed = 20f;
    [BoxGroup("Settings"),SerializeField] private float lifeTime = 1f;
    [BoxGroup("Settings"),SerializeField] private float Acceleration = 0;
    private DamageOnTouch damageOnTouch;
    private Character owner;

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
        damageOnTouch.AddToIgnoreList(owner.gameObject);
        damageOnTouch.IgnoreTeamMembers();
        currentSpeed = speed;
        // damageOnTouch.health?.ResetHealth();
    }

    public void OnDespawn()
    {
        damageOnTouch.ClearIgnoreList();
    }

    public Projectile SetOwner(Character o)
    {
        owner = o;
        damageOnTouch.SetOwner(owner);
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