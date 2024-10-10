using Unity.Netcode;
using UnityEngine;

public abstract class CharacterAbility : NetworkBehaviour
{
    [SerializeField] private CharacterStates.CharacterAbility abilityType;
    protected Character character;
    protected ICharacterController controller;

    protected virtual void Awake() { PreInitialization(); }

    private void PreInitialization() { character = GetComponentInParent<Character>(); }

    protected virtual void Start()
    {
        controller = character.Controller;
        Initialization();
    }

    protected virtual void Initialization() { }

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