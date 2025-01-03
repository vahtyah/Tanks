﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] protected GameObject abilityNode;
    public ICharacterController Controller;
    protected readonly List<CharacterAbility>  characterAbilities = new();
    public Health Health { get; private set; }
    
    //StateMachine
    public StateMachine<CharacterStates.CharacterCondition> conditionState { get; private set; } //TODO: chua biet lam gi
    protected virtual void Awake()
    {
        Controller = GetComponent<ICharacterController>();
        Health = GetComponent<Health>();
        Initialization();
    }
    
    protected virtual void Initialization()
    {
        conditionState = new StateMachine<CharacterStates.CharacterCondition>(gameObject);
        characterAbilities.AddRange(abilityNode.GetComponents<CharacterAbility>());
    }
    
    protected virtual void ProcessAbilities()
    {

    }
    
    protected virtual void FixedProcessAbilities()
    {
        
    }
}
