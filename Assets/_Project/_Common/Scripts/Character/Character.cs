using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] protected GameObject abilityNode;
    public ICharacterController Controller { get; private set; }
    public Health Health { get; private set; }
    
    protected readonly List<CharacterAbility>  characterAbilities = new();
    public PhotonView PhotonView { get; private set; }
    
    //StateMachine
    public StateMachine<CharacterStates.CharacterCondition> conditionState { get; private set; } //TODO: chua biet lam gi
    protected virtual void Awake()
    {
        Controller = GetComponent<ICharacterController>();
        Health = GetComponent<Health>();
        PhotonView = GetComponent<PhotonView>();
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
