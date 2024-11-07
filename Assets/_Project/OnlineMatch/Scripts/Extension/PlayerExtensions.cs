using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

public static class PlayerExtensions
{
    public static void SetReadyInLobby(this Player player, bool ready)
    {
        player.SetCustomProperties(new Hashtable { { GlobalString.PLAYER_READY_ENTRY, ready } });
    }
    
    public static bool IsReadyInLobby(this Player player)
    {
        if (!player.CustomProperties.TryGetValue(GlobalString.PLAYER_READY_ENTRY, out var isReady))
        {
            return false;
        }

        return (bool)isReady;
    }
    public static void SetReadyInGame(this Player player, bool ready)
    {
        player.SetCustomProperties(new Hashtable { { GlobalString.PLAYER_READY_INGAME, ready } });
    }

    public static bool IsReadyInGame(this Player player)
    {
        if (!player.CustomProperties.TryGetValue(GlobalString.PLAYER_READY_INGAME, out var isReady))
        {
            return false;
        }

        return (bool)isReady;
    }

    public static Team AssignTeam(this Player player, TeamType teamType)
    {
        var team = Team.GetTeamByType(teamType);
        if (team == null)
        {
            Debug.LogWarning($"Team {teamType} does not exist");
            return null;
        }

        player.SetCustomProperties(new Hashtable { { GlobalString.TEAM, teamType } });
        // team.AddPlayer(player);

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
        return Team.GetTeamByType(teamType);
    }

    public static void AddScore(this Player player, int score)
    {
        player.SetCustomProperties(new Hashtable
        {
            { GlobalString.PLAYER_SCORE, (int)player.CustomProperties[GlobalString.PLAYER_SCORE] + score }
        });
    }

    public static int GetScore(this Player player)
    {
        return player.CustomProperties.TryGetValue(GlobalString.PLAYER_SCORE, out var score) ? (int)score : 0;
    }

    public static void AddKill(this Player player, int skill)
    {
        player.SetCustomProperties(new Hashtable
        {
            { GlobalString.PLAYER_KILLS, (int)player.CustomProperties[GlobalString.PLAYER_KILLS] + skill }
        });
    }

    public static int GetKills(this Player player)
    {
        return player.CustomProperties.TryGetValue(GlobalString.PLAYER_KILLS, out var score) ? (int)score : 0;
    }

    public static void AddDeath(this Player player, int death)
    {
        player.SetCustomProperties(new Hashtable
        {
            { GlobalString.PLAYER_DEATHS, (int)player.CustomProperties[GlobalString.PLAYER_DEATHS] + death }
        });
    }

    public static int GetDeaths(this Player player)
    {
        return player.CustomProperties.TryGetValue(GlobalString.PLAYER_DEATHS, out var score) ? (int)score : 0;
    }
}