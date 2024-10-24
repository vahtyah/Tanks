using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;

public enum TeamType
{
    Red,
    Blue,
    Green,
    Yellow
}

public class Team
{
    private static TeamManager manager;

    public TeamType TeamType;
    public Color TeamColor;
    public HashSet<Player> Players { get; private set; } = new();
    
    private Action<Player, Team> onPlayerJoin;
    private Action<Player, Team> onPlayerLeft;

    public static Team Register(TeamType teamType)
    {
        if (manager == null)
        {
            manager = TeamManager.Instance ?? new GameObject("TeamManager").AddComponent<TeamManager>();
        }

        var team = new Team
        {
            TeamType = teamType,
            TeamColor = teamType switch
            {
                TeamType.Red => Color.red,
                TeamType.Blue => Color.blue,
                TeamType.Green => Color.green,
                TeamType.Yellow => Color.yellow,
                _ => Color.white
            }
        };
        manager.Register(team);
        return team;
    }
    
    public static Team TryGetTeamBy(TeamType teamType)
    {
        return manager.TryGetTeam(teamType, out var team) ? team : null;
    }
    
    public Team OnPlayerJoin(Action<Player, Team> onPlayerJoin)
    {
        this.onPlayerJoin += onPlayerJoin;
        return this;
    }
    
    public Team OnPlayerLeft(Action<Player, Team> onPlayerLeft)
    {
        this.onPlayerLeft += onPlayerLeft;
        return this;
    }

    public void AddPlayerToTeam(Player player)
    {
        Players.Add(player);
        onPlayerJoin?.Invoke(player, this);
    }

    public void RemovePlayerFromTeam(Player player)
    {
        Players.Remove(player);
        onPlayerLeft?.Invoke(player, this);
    }
}

public class TeamManager : SingletonPunCallbacks<TeamManager>
{
    public Dictionary<TeamType, Team> teams = new();
    public const string TeamPlayerProp = "Team";

    public void Register(Team team)
    {
        PhotonNetwork.PlayerList[0].GetPhotonTeam();
        if (teams.TryAdd(team.TeamType, team)) return;
        Debug.LogWarning($"Team {team.TeamType} already exists");
    }

    public override void OnJoinedRoom()
    {
        UpdateTeams();
        PhotonNetwork.LocalPlayer.SetTeam(GetLowestTeam());
    }

    public override void OnLeftRoom()
    {
        teams.Clear();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        var team = newPlayer.GetTeam();
        team?.AddPlayerToTeam(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        var team = otherPlayer.GetTeam();
        team?.RemovePlayerFromTeam(otherPlayer);
    }
    
    private TeamType GetLowestTeam()
    {
        var lowestTeam = TeamType.Red;
        var lowestCount = int.MaxValue;
        foreach (var team in teams.Where(team => team.Value.Players.Count < lowestCount))
        {
            lowestCount = team.Value.Players.Count;
            lowestTeam = team.Key;
        }
        return lowestTeam;
    }

    private void UpdateTeams()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            var team = player.GetTeam();
            team?.AddPlayerToTeam(player);
        }
    }

    public bool TryGetTeam(TeamType teamType, out Team team)
    {
        return teams.TryGetValue(teamType, out team);
    }
}

public static class TeamExtensions
{
    public static void SetTeam(this Player player, TeamType teamType)
    {
        var team = Team.TryGetTeamBy(teamType);
        if (team == null)
        {
            Debug.LogWarning($"Team {teamType} does not exist");
            return;
        }

        player.SetCustomProperties(new Hashtable { { TeamManager.TeamPlayerProp, teamType } });
        team.AddPlayerToTeam(player);
    }

    public static Team GetTeam(this Player player)
    {
        return player.CustomProperties.TryGetValue(TeamManager.TeamPlayerProp, out var teamName)
            ? Team.TryGetTeamBy((TeamType)teamName)
            : null;
    }
}