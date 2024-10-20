using System.Linq;
using Photon.Pun;
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
        if(!PhotonView.IsMine) return;
        foreach (var ability in characterAbilities.Where(ability => ability.enabled))
        {
            ability.ProcessAbility();
        }
    }

    protected override void FixedProcessAbilities()
    {
        if(!PhotonView.IsMine) return;
        foreach (var ability in characterAbilities.Where(ability => ability.enabled))
        {
            ability.FixedProcessAbility();
        }
    }
}
