using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using InfoGame;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;

public enum TeamType
{
    Red,
    Blue,
    Green,
    Yellow,
    None
}

[Serializable]
public class Team
{
    private static RoomManager manager;

    public TeamType TeamType;
    public Color TeamColor;
    public List<Player> Players { get; private set; } = new();

    private Action<Player, Team> onPlayerJoin;
    private Action<Player, Team> onPlayerLeft;

    public static Team Register(TeamType teamType)
    {
        EnsureManagerExists();

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
        manager.RegisterTeam(team);
        return team;
    }

    public static void CacheTeam(Character playerCharacter, TeamType teamType)
    {
        EnsureManagerExists();
        manager.CacheTeam(playerCharacter, teamType);
    }

    public static TeamType TryGetCacheTeam(Character playerCharacter)
    {
        EnsureManagerExists();
        return manager.teamCache.GetValueOrDefault(playerCharacter, TeamType.None);
    }

    public static Team TryGetTeamBy(PlayerCharacter playerCharacter)
    {
        EnsureManagerExists();
        return manager.teamCache.TryGetValue(playerCharacter, out var teamType) ? TryGetTeamBy(teamType) : null;
    }

    public static Team TryGetTeamBy(TeamType teamType)
    {
        EnsureManagerExists();
        return manager.TryGetTeam(teamType, out var team) ? team : null;
    }

    public static Team TryGetTeamBy(Player player)
    {
        EnsureManagerExists();
        return TryGetTeamBy((TeamType)player.CustomProperties[GlobalString.TEAM]);
    }

    public static List<Player> GetPlayersBy(TeamType teamType)
    {
        EnsureManagerExists();
        return TryGetTeamBy(teamType)?.Players.ToList();
    }

    public static List<Team> GetTeams()
    {
        EnsureManagerExists();
        return manager.Teams.Values.ToList();
    }

    private static void EnsureManagerExists()
    {
        if (manager == null)
        {
            manager = RoomManager.Instance ?? new GameObject("TeamManager").AddComponent<RoomManager>();
        }
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

public class RoomManager : PersistentSingletonPunCallbacks<RoomManager>
{
    public Info roomNameInfo;
    public Info teamNameInfo;
    public List<Team> teamsDebug = new(); //TODO: Remove this
    public Dictionary<int, PlayerCharacter> PlayerCharacters { get; private set; } = new();
    [ShowInInspector]
    public Dictionary<Character, TeamType> teamCache = new();

    public void CacheTeam(Character playerCharacter, TeamType teamType)
    {
        teamCache[playerCharacter] = teamType;
    }

    public Dictionary<TeamType, Team> Teams { get; private set; } = new();

    private void Start()
    {
        roomNameInfo = Info.Register("RoomManager", "Room");
        teamNameInfo = Info.Register("RoomManager", "Team");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Debug.Log("Team.TryGetTeamBy(Red) = " + Team.TryGetTeamBy(TeamType.Red));

        if (Input.GetKeyDown(KeyCode.P))
        {
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GlobalString.TEAM_SIZE, out var teamSize);
            Debug.Log("teamSize = " + (int)teamSize);
        }

        roomNameInfo.SetValue(PhotonNetwork.CurrentRoom?.Name);
        var team = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(GlobalString.TEAM, out var teamType)
            ? teamType
            : "None";
        teamNameInfo.SetValue(team.ToString());
    }

    public void RegisterTeam(Team team)
    {
        if (Teams.TryAdd(team.TeamType, team))
        {
            teamsDebug.Add(team);
            return;
        }

        Debug.LogWarning($"Team {team.TeamType} already exists");
    }

    public override void OnCreatedRoom()
    {
        RoomEvent.Trigger(RoomEventType.RoomCreate, PhotonNetwork.CurrentRoom);
    }

    public override void OnJoinedRoom()
    {
        UpdateTeams();
        PhotonNetwork.LocalPlayer.SetTeam(GetLowestTeam());
        RoomEvent.Trigger(RoomEventType.RoomJoin, PhotonNetwork.CurrentRoom);
    }

    public override void OnLeftRoom()
    {
        Teams.Clear();
        RoomEvent.Trigger(RoomEventType.RoomLeave, PhotonNetwork.CurrentRoom);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        var team = otherPlayer.GetTeam();
        team?.RemovePlayerFromTeam(otherPlayer);
        RoomEvent.Trigger(RoomEventType.PlayerLeft, PhotonNetwork.CurrentRoom);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.TryGetValue(GlobalString.TEAM, out var teamType))
        {
            var team = Team.TryGetTeamBy((TeamType)teamType);
            team?.AddPlayerToTeam(targetPlayer);
        }
    }

    private TeamType GetLowestTeam()
    {
        var lowestTeam = TeamType.Red;
        var lowestCount = int.MaxValue;
        foreach (var team in Teams.Where(team => team.Value.Players.Count < lowestCount))
        {
            lowestCount = team.Value.Players.Count;
            lowestTeam = team.Key;
        }

        return lowestTeam;
    }

    private void UpdateTeams()
    {
        if (Teams.Count > 0) return;
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GlobalString.TEAM_SIZE, out var teamSiz);
        if (teamSiz != null) Debug.Log("teamSize = " + (int)teamSiz);
        else
        {
            Debug.LogWarning("Team size is not set");
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GlobalString.TEAM_SIZE, out var teamSize) &&
            teamSize is int size)
        {
            for (int i = 0; i < size; i++)
            {
                Team.Register((TeamType)i);
            }
        }


        var players = PhotonNetwork.CurrentRoom.Players.Values;
        foreach (var player in players)
        {
            var team = player.GetTeam();
            if (team != null)
            {
                team.AddPlayerToTeam(player);
            }
        }
    }

    public bool TryGetTeam(TeamType teamType, out Team team)
    {
        return Teams.TryGetValue(teamType, out team);
    }
}

public static class TeamExtensions
{
    public static List<Team> CreateTeams(this Room room, int teamSize)
    {
        var teams = new List<Team>();
        for (int i = 0; i < teamSize; i++)
        {
            teams.Add(Team.Register((TeamType)i));
        }

        return teams;
    }

    public static Team SetTeam(this Player player, TeamType teamType)
    {
        var team = Team.TryGetTeamBy(teamType);
        if (team == null)
        {
            Debug.LogWarning($"Team {teamType} does not exist");
            return null;
        }

        player.SetCustomProperties(new Hashtable { { GlobalString.TEAM, teamType } });
        // team.AddPlayerToTeam(player);
        return team;
    }

    public static Team GetTeam(this Player player)
    {
        if (!player.CustomProperties.TryGetValue(GlobalString.TEAM, out var teamName))
        {
            Debug.LogWarning("Player does not have a team");
            return null;
        }

        var teamType = (TeamType)teamName;
        return Team.TryGetTeamBy(teamType);
    }

    public static TeamType GetTeam(this Character player)
    {
        var teamType = Team.TryGetCacheTeam(player);
        if (teamType != TeamType.None) return teamType;
        
        var team = player.PhotonView.Owner.GetTeam();
        teamType = team?.TeamType ?? TeamType.None;
        Team.CacheTeam(player, teamType);

        return teamType;
    }
}