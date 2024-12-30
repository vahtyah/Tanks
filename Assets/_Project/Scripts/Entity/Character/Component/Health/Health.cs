using System;
using ExitGames.Client.Photon;
using MoreMountains.Feedbacks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Health : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] protected bool disableColliderOnDeath = true;
    [SerializeField] private MMF_Player onHitFeedbacks;
    public bool IsInvulnerable;

    private float currentHealth;
    public Action onDeath;
    private Action<float> onHit { get; set; }
    private Collider col;
    protected PhotonView PhotonView { get; private set; }
    private int lastHitBy;

    protected virtual void Awake()
    {
        col = GetComponent<Collider>();
        PhotonView = GetComponent<PhotonView>();
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
        set
        {
            maxHealth = Mathf.Max(value, 0);
            Reset(); // Reset health if max health changes
        }
    }

    public bool TakeDamage(float damage, Character lastHitBy)
    {
        if (IsInvulnerable) return false;

        if (PhotonView.IsMine)
        {
            onHitFeedbacks?.PlayFeedbacks();
            PhotonView.RPC(nameof(TakeDamageRPC), RpcTarget.All, damage, lastHitBy.PhotonView.ViewID);
            UnityEngine.Debug.Log($"Health: {CurrentHealth} in TakeDamage");
            if(CurrentHealth <= 0)
                return true;
        }

        return false;
    }

    [PunRPC]
    public void TakeDamageRPC(float damage, int lastHitBy)
    {
        this.lastHitBy = lastHitBy;
        CurrentHealth -= damage;
        UnityEngine.Debug.Log($"Health: {CurrentHealth} in TakeDamageRPC");
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