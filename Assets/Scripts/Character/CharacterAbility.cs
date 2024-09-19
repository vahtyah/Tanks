using UnityEngine;

public abstract class CharacterAbility : MonoBehaviour
{
    protected Character character;
    protected CharacterInput characterInput;
    protected TankController controller;

    public bool AbilityInitialized { get; private set; }
    protected virtual void Awake()
    {
        PreInitialization();
    }

    private void PreInitialization()
    {
        character = GetComponentInParent<Character>();
        controller = GetComponentInParent<TankController>();
    }
    
    protected virtual void Start()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        characterInput = character.CharacterInput;
        AbilityInitialized = true;
    }

    public virtual void ProcessAbility()
    {
        // Process ability
    }
    
    public virtual void FixedProcessAbility()
    {
        // Process ability
    }

    protected virtual void HandleInput()
    {
        
    }
}
