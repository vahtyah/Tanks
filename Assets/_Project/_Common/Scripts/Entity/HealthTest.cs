using System;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class HealthTest : MonoBehaviour
{

    [SerializeField, BoxGroup("Health Settings")]
    private float maxHealth = 100;

    [SerializeField, BoxGroup("Health Settings")]
    protected bool isInvulnerable;

    [SerializeField, BoxGroup("Feedbacks")]
    protected MMFeedbacks deathFeedbacks;

    [SerializeField, BoxGroup("Death Settings")]
    private float destructionDelay = 0f;

    [SerializeField, HorizontalGroup("Death Settings" + "/model", DisableAutomaticLabelWidth = true)]
    private bool disableModelOnDeath = false;

    [HideLabel]
    [HorizontalGroup("Death Settings" + "/model")]
    [EnableIf("disableModelOnDeath")]
    [SerializeField]
    [LabelWidth(50)]
    [SuffixLabel("model ")]
    private GameObject model;

    [SerializeField, BoxGroup("Death Settings")]
    private bool disableControllerOnDeath = false;

    [SerializeField, BoxGroup("Death Settings")]
    private bool disableCollisionOnDeath = false;

    [Debug] private float currentHealth;
    private Action<float> OnHealthChanged { get; set; }
    private Collider col;
    private Entity entity;

    public bool IsInvulnerable
    {
        get => isInvulnerable;
        protected set => isInvulnerable = value;
    }

    protected virtual void Awake()
    {
        col = GetComponent<Collider>();
        entity = GetComponent<Entity>();
    }

    private void OnEnable()
    {
        ResetHealth();
        if (disableModelOnDeath && model) model.SetActive(true);
        if (disableCollisionOnDeath) col.enabled = true;
        if (disableControllerOnDeath && entity) entity.enabled = true;
    }

    private void Start()
    {
        ResetHealth();
    }

    public float CurrentHealth
    {
        get => currentHealth;
        protected set
        {
            if (IsInvulnerable) return;
            float newHealth = Mathf.Clamp(value, 0, MaxHealth);
            // if (Mathf.Approximately(currentHealth, newHealth)) return;

            currentHealth = newHealth;
            OnHealthChanged?.Invoke(GetHealthPercentage());

            if (currentHealth <= 0)
            {
                HandleDeath();
            }
        }
    }

    protected virtual void HandleDeath()
    {
        if (disableModelOnDeath && model) model.SetActive(false);
        if (disableCollisionOnDeath) col.enabled = false;
        if (disableControllerOnDeath && entity) entity.enabled = false;
        deathFeedbacks?.PlayFeedbacks();
        OnDeath();
        Invoke(nameof(DestroyEntity), destructionDelay);
    }

    protected float MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = Mathf.Max(value, 0);
            ResetHealth(); // Reset health if max health changes
        }
    }

    protected virtual void DestroyEntity()
    {
    }

    public abstract void TakeDamage(float damage, Character lastHitBy);
    public abstract void OnDeath();
    private float GetHealthPercentage() => (float)CurrentHealth / MaxHealth;

    public virtual void ResetHealth() => CurrentHealth = MaxHealth;

    public void AddHealthChangeListener(Action<float> onHitAction) => OnHealthChanged += onHitAction;
}