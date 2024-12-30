using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BoardTeamPanel : MonoBehaviour
{
    [SerializeField] private GameObject teamBoardPrefab;
    [SerializeField] private Transform teamContainer;
    private Dictionary<Team, TeamBoard> teamBoards = new();

    public void Initialize(List<Team> teams)
    {
        foreach (var team in teams)
        {
            team.OnPlayerJoin(AddPlayer);
            team.OnPlayerLeft(RemovePlayer);
            var teamBoard = TeamBoard.Create(teamBoardPrefab, teamContainer, team);
            teamBoards.Add(team, teamBoard);
            team.Players.ForEach(player => AddPlayer(player, team));
        }
    }

    public void AddPlayer(Player player, Team team)
    {
        if (this != null)
            teamBoards[team].AddMember(player, (int)team.TeamType % 2 != 0);
    }

    public void RemovePlayer(Player player, Team team)
    {
        if (this != null)
            teamBoards[team].RemoveMember(player);
    }

    /*public void JoinRoom()
    {
        var teams = Team.GetAllTeams();
        Initialize(teams.Count);
        teams.ForEach(team => team.Players.ForEach(player => AddPlayer(player, team)));
    }*/
}