using System.Collections.Generic;
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
        if (PhotonView.IsMine)
        {
            ProcessAbilities();
        }
    }

    private void FixedUpdate()
    {
        if (PhotonView.IsMine)
        {
            FixedProcessAbilities();
        }
    }

    private void ProcessAbilities()
    {
        foreach (var ability in abilities)
        {
            ability.ProcessAbility();
        }
    }

    private void FixedProcessAbilities()
    {
        foreach (var ability in abilities)
        {
            ability.FixedProcessAbility();
        }
    }

    public void SetTeam(TeamType team)
    {
        Team = team;
    }
}