using System.Linq;
using UnityEngine;

public class PlayerCharacter : Character
{
    [SerializeField] private string playerID = "Player1";
    public GameObject CameraTarget;
    
    public string PlayerID => playerID;
    
    private void Update()
    {
        ProcessAbilities();
    }
    
    private void FixedUpdate()
    {
        FixedProcessAbilities();
    }

    protected override void ProcessAbilities()
    {
        base.ProcessAbilities();
        foreach (var ability in characterAbilities.Where(ability => ability.enabled && ability.AbilityInitialized))
        {
            ability.ProcessAbility();
        }
    }

    protected override void FixedProcessAbilities()
    {
        base.FixedProcessAbilities();
        foreach (var ability in characterAbilities.Where(ability => ability.enabled && ability.AbilityInitialized))
        {
            ability.FixedProcessAbility();
        }
    }
    
    
}
