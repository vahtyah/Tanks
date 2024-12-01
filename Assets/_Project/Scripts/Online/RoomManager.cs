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
    [Log] public List<Player> Players { get; private set; } = new();

    private Action<Player, Team> onPlayerJoin;
    private Action<Player, Team> onPlayerLeft;

    public static Team Create(TeamType teamType)
    {
        EnsureManagerExists();

        var team = new Team
        {
            TeamType = teamType,
            TeamColor = GetColorByTeamType(teamType)
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
        return manager.TeamCharacters.GetValueOrDefault(playerCharacter, TeamType.None);
    }

    public static Team GetTeamByCharacter(PlayerCharacter playerCharacter)
    {
        EnsureManagerExists();
        return manager.TeamCharacters.TryGetValue(playerCharacter, out var teamType) ? GetTeamByType(teamType) : null;
    }

    public static Team GetTeamByType(TeamType teamType)
    {
        EnsureManagerExists();
        return manager.TryGetTeam(teamType, out var team) ? team : null;
    }

    public static Team GetTeamByPlayer(Player player)
    {
        EnsureManagerExists();
        return GetTeamByType(player.GetTeam().TeamType);
    }

    public static List<Player> GetPlayersByTeam(TeamType teamType)
    {
        EnsureManagerExists();
        return GetTeamByType(teamType)?.Players.ToList();
    }

    public static List<Team> GetAllTeams()
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

    public void AddPlayer(Player player)
    {
        Players.Add(player);
        onPlayerJoin?.Invoke(player, this);
    }

    public void RemovePlayerFromTeam(Player player)
    {
        Players.Remove(player);
        onPlayerLeft?.Invoke(player, this);
    }
    
    private static Color GetColorByTeamType(TeamType teamType)
    {
        return teamType switch
        {
            TeamType.Red => Color.red,
            TeamType.Blue => Color.blue,
            TeamType.Green => Color.green,
            TeamType.Yellow => Color.yellow,
            _ => Color.white
        };
    }
}

public class RoomManager : PersistentSingletonPunCallbacks<RoomManager>
{
    public Info roomNameInfo;
    public Info teamNameInfo;
    [Log] public Dictionary<Character, TeamType> TeamCharacters = new();
    [Log] public Dictionary<TeamType, Team> Teams { get; private set; } = new();

    private void Start()
    {
        roomNameInfo = Info.Register("RoomManager", "Room");
        teamNameInfo = Info.Register("RoomManager", "Team");
    }

    private void Update()
    {
        roomNameInfo.SetValue(PhotonNetwork.CurrentRoom?.Name);
        var team = PhotonNetwork.LocalPlayer.CustomProperties.GetValueOrDefault(GlobalString.TEAM, "None");
        teamNameInfo.SetValue(team.ToString());
    }

    public void CacheTeam(Character playerCharacter, TeamType teamType)
    {
        TeamCharacters[playerCharacter] = teamType;
    }

    public void RegisterTeam(Team team)
    {
        if (!Teams.TryAdd(team.TeamType, team))
        {
            UnityEngine.Debug.LogWarning($"Team {team.TeamType} already exists");
        }
    }

    public override void OnCreatedRoom()
    {
        RoomEvent.Trigger(RoomEventType.RoomCreate, PhotonNetwork.CurrentRoom);
    }

    public override void OnJoinedRoom()
    {
        InitializeTeams();
        PhotonNetwork.LocalPlayer.AssignTeam(GetTeamWithFewestPlayers());
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
        RoomEvent.Trigger(RoomEventType.PlayerLeft, otherPlayer);
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomEvent.Trigger(RoomEventType.PlayerJoin, newPlayer);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.TryGetValue(GlobalString.TEAM, out var teamType))
        {
            var team = Team.GetTeamByType((TeamType)teamType);
            team?.AddPlayer(targetPlayer);
        }
    }

    private TeamType GetTeamWithFewestPlayers()
    {
        return Teams.OrderBy(t => t.Value.Players.Count).First().Key;
    }

    private void InitializeTeams()
    {
        if (Teams.Count > 0) return;
        var teamSiz = PhotonNetwork.CurrentRoom.GetTeamSize();
        if (teamSiz != -1)
        {
            for (int i = 0; i < teamSiz; i++)
            {
                Team.Create((TeamType)i);
            }
        }

        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            var team = player.GetTeam();
            team?.AddPlayer(player);
        }
    }

    public bool TryGetTeam(TeamType teamType, out Team team)
    {
        return Teams.TryGetValue(teamType, out team);
    }
}