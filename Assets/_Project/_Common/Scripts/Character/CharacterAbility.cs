using Photon.Pun;
using UnityEngine;

public abstract class CharacterAbility : MonoBehaviour
{
    [SerializeField] private CharacterStates.CharacterAbility abilityType;
    protected Character character;
    protected ICharacterController controller;
    protected PhotonView photonView;

    protected virtual void Awake() { PreInitialization(); }

    protected virtual void PreInitialization()
    {
        character = GetComponentInParent<Character>();
        controller = character.Controller;
        photonView = character.PhotonView;
    }

    protected virtual void Start() { Initialization(); }

    protected virtual void Initialization() { }

    public virtual void ProcessAbility()
    {
        // Process ability
    }

    public virtual void FixedProcessAbility()
    {
        // Process ability
    }
    public virtual void FixedLagCompensation() { }
    public virtual void LagCompensation() { }

    protected virtual void HandleInput() { }

    public CharacterStates.CharacterAbility AbilityType => abilityType;
}