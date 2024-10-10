using System;
using MoreMountains.Feedbacks;
using Unity.Netcode;
using UnityEngine;

public abstract class Health : NetworkBehaviour
{
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] protected bool disableColliderOnDeath = true;
    [SerializeField] protected float currentHealth;

    protected Character character;
    public Action onDeath;
    private Action<float> onHit { get; set; }
    private Collider col;

    protected virtual void Awake()
    {
        col = GetComponent<Collider>();
        character = GetComponent<Character>();
    }

    protected virtual void Start() { Reset(); }

    protected float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            onHit?.Invoke(GetHealthAmountNormalized());
            if (currentHealth <= 0)
                Die();
        }
    }

    protected float MaxHealth
    {
        get => maxHealth;
        private set => Mathf.Max(value, 0);
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage; // Gọi phương thức giảm sức khỏe trên server
    }

    protected void Die()
    {
        //Feedback, particles, etc.
        if (disableColliderOnDeath)
        {
            col.enabled = false;
        }

        onDeath?.Invoke();
        OnDeath();
    }

    protected abstract void OnDeath();

    public void Reset() { CurrentHealth = MaxHealth; }

    public float GetHealthAmountNormalized() => (float)CurrentHealth / MaxHealth;
    public void AddOnDeathListener(Action _onDeath) { onDeath += _onDeath; }
    public void RemoveOnDeathListener(Action _onDeath) { onDeath -= _onDeath; }
    public void AddOnHitListener(Action<float> _onHit) { onHit += _onHit; }
    public void RemoveOnHitListener(Action<float> _onHit) { onHit -= _onHit; }
}