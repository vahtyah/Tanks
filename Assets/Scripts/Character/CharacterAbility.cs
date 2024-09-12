using UnityEngine;

public class CharacterAbility : MonoBehaviour
{
    protected Character character;
    protected InputManager inputManager;
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
        inputManager = character.InputManager;
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
