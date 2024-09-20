using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private string playerID = "Player1";
    [SerializeField] private GameObject abilityNode;
    public CharacterInput CharacterInput { get; private set; }
    readonly List<CharacterAbility>  characterAbilities = new();
    
    //StateMachine
    public StateMachine<CharacterStates.CharacterCondition> conditionState { get; private set; }
    public GameObject CameraTarget;
    private void Awake()
    {
        CharacterInput = GetComponent<CharacterInput>();
        Initialization();
    }
    
    private void Initialization()
    {
        conditionState = new StateMachine<CharacterStates.CharacterCondition>(gameObject);
        characterAbilities.AddRange(abilityNode.GetComponents<CharacterAbility>());
    }
    
    private void Update()
    {
        ProcessAbilities();
    }
    
    private void FixedUpdate()
    {
        FixedProcessAbilities();
    }

    private void ProcessAbilities()
    {
        foreach (var ability in characterAbilities.Where(ability => ability.enabled && ability.AbilityInitialized))
        {
            ability.ProcessAbility();
        }
    }
    
    private void FixedProcessAbilities()
    {
        foreach (var ability in characterAbilities.Where(ability => ability.enabled && ability.AbilityInitialized))
        {
            ability.FixedProcessAbility();
        }
    }
    

    public string PlayerID => playerID;
}
