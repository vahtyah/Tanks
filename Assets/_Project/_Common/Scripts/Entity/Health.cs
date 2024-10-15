using System;
using ExitGames.Client.Photon;
using MoreMountains.Feedbacks;
using Photon.Pun;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] protected bool disableColliderOnDeath = true;
    [SerializeField] protected float currentHealth;

    public Action onDeath;
    private Action<float> onHit { get; set; }
    private Collider col;
    public PhotonView photonView;
    private int lastHitBy;

    protected virtual void Awake()
    {
        col = GetComponent<Collider>();
        photonView = GetComponent<PhotonView>();
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

    public void TakeDamage(float damage, Character lastHitBy)
    {
        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.Others, damage, lastHitBy.PhotonView.ViewID);
    }

    [PunRPC]
    public void TakeDamageRPC(float damage, int lastHitBy)
    {
        this.lastHitBy = lastHitBy;
        if (photonView.IsMine)
            CurrentHealth -= damage;
    }

    private void Die()
    {
        //Feedback, particles, etc.
        if (disableColliderOnDeath)
        {
            col.enabled = false;
        }
        onDeath?.Invoke();
        OnDeath(lastHitBy);
    }

    protected abstract void OnDeath(int lastHitBy);
    public void Reset() { CurrentHealth = MaxHealth; }
    public float GetHealthAmountNormalized() => (float)CurrentHealth / MaxHealth;
    public void AddOnDeathListener(Action _onDeath) { onDeath += _onDeath; }
    public void RemoveOnDeathListener(Action _onDeath) { onDeath -= _onDeath; }
    public void AddOnHitListener(Action<float> _onHit) { onHit += _onHit; }
    public void RemoveOnHitListener(Action<float> _onHit) { onHit -= _onHit; }
}