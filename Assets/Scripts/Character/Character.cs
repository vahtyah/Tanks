﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private string playerID = "Player1";
    [SerializeField] private GameObject abilityNode;
    public CharacterInput CharacterInput { get; private set; }
    readonly List<CharacterAbility>  characterAbilities = new();
    
    private void Awake()
    {
        CharacterInput = GetComponent<CharacterInput>();
        InitializeAbilities();
    }
    
    private void InitializeAbilities()
    {
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
