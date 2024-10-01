using UnityEngine;

public abstract class CharacterAbility : MonoBehaviour
{
    [SerializeField] private CharacterStates.CharacterAbility abilityType;
    protected Character character;
    protected ICharacterController controller;

    public bool AbilityInitialized { get; private set; }
    protected virtual void Awake() { PreInitialization(); }

    private void PreInitialization()
    {
        character = GetComponentInParent<Character>();
    }

    protected virtual void Start()
    {
        controller = character.Controller;
        Initialization();
    }

    protected virtual void Initialization() { AbilityInitialized = true; }

    public virtual void ProcessAbility()
    {
        // Process ability
    }

    public virtual void FixedProcessAbility()
    {
        // Process ability
    }

    protected virtual void HandleInput() { }

    public CharacterStates.CharacterAbility AbilityType => abilityType;
}