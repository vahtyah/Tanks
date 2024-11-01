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
        var enabledAbilities = abilities.Where(ability => ability.enabled).ToList();
        foreach (var ability in enabledAbilities)
        {
            ability.ProcessAbility();
        }
    }

    private void FixedProcessAbilities()
    {
        var enabledAbilities = abilities.Where(ability => ability.enabled).ToList();
        foreach (var ability in enabledAbilities)
        {
            ability.FixedProcessAbility();
        }
    }

    public void SetTeam(TeamType team)
    {
        Team = team;
    }
}