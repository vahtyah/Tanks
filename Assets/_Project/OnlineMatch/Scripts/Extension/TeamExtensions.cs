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
}