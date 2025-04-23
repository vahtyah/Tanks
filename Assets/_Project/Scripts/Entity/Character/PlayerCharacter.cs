using Photon.Pun;
using UnityEngine;

public class PlayerCharacter : Character, IController
{
    [SerializeField] private string playerID = "Player1";
    public Transform CameraTarget { get; private set; }

    protected override void Initialize()
    {
        base.Initialize();
        CameraTarget = Model.CameraTarget;
    }

    public string PlayerID => playerID;
    
    private void Update()
    {
        if (PhotonView.IsMine || PhotonNetwork.OfflineMode)
        {
            ProcessAbilities();
        }
    }

    private void FixedUpdate()
    {
        if (PhotonView.IsMine || PhotonNetwork.OfflineMode)
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