﻿using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class CharacterAbility : MonoBehaviour
{
    [SerializeField] private CharacterStates.CharacterAbility abilityType;
    protected Character Character;
    protected ICharacterController Controller;
    protected PhotonView PhotonView;

    protected virtual void Awake()
    {
        PreInitialize();
    }

    protected virtual void PreInitialize()
    {
        Character = GetComponentInParent<Character>();
        PhotonView = Character.PhotonView;
        Controller = Character.Controller;
    }

    protected virtual void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
    }

    public virtual void ProcessAbility()
    {
        // Process ability
    }

    public virtual void FixedProcessAbility()
    {
        // Process ability
    }

    public virtual void FixedLagCompensation()
    {
    }

    public virtual void LagCompensation()
    {
    }

    protected virtual void HandleInput()
    {
    }

    public CharacterStates.CharacterAbility AbilityType => abilityType;
}