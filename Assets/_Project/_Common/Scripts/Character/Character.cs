using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] protected GameObject abilityNode;
    public ICharacterController Controller { get; private set; }
    public Health Health { get; private set; }

    protected readonly List<CharacterAbility> abilities = new();
    public PhotonView PhotonView { get; private set; }
    public Collider Collider { get; private set; }
    public Weapon EquippedWeapon { get; private set; }
    public TeamType Team { get; protected set; }
    public List<Character> Teammates { get; protected set; } = new();

    //StateMachine
    public StateMachine<CharacterStates.CharacterCondition>
        ConditionState { get; private set; } //TODO: chua biet lam gi

    protected virtual void Awake()
    {
        Controller = GetComponent<ICharacterController>();
        Health = GetComponent<Health>();
        PhotonView = GetComponent<PhotonView>();
        Collider = GetComponent<Collider>();
        Initialize();
    }

    protected virtual void Initialize()
    {
        ConditionState = new StateMachine<CharacterStates.CharacterCondition>(gameObject);
        abilities.AddRange(abilityNode.GetComponents<CharacterAbility>());
    }


    public void EquipWeapon(Weapon weapon)
    {
        EquippedWeapon = weapon;
    }
}