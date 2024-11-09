using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

public static class TeamExtensions
{
    public static int GetTeamScore(this Team team)
    {
        var result = 0;
        foreach (var player in team.Players)
        {
            result += player.GetScore();
        }

        return result;
    }
     
    public static List<Player> GetPlayersExcept(this Team team, Player player)
    {
        var result = new List<Player>();
        foreach (var teamPlayer in team.Players)
        {
            if (Equals(teamPlayer, player))
            {
                continue;
            }
            result.Add(teamPlayer);
        }

        return result;
    }
}