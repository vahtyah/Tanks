using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

public static class RoomExtensions
{
    public static List<Team> CreateTeams(this Room room, int teamSize)
    {
        var teams = new List<Team>();
        for (int i = 0; i < teamSize; i++)
        {
            teams.Add(Team.Create((TeamType)i));
        }

        return teams;
    }
    public static void SetTeamSize(this Room room, int teamSize)
    {
        room.SetCustomProperties(new Hashtable
        {
            {GlobalString.TEAM_SIZE, teamSize}
        });
    }
    
    public static int GetTeamSize(this Room room)
    {
        if (room.CustomProperties.TryGetValue(GlobalString.TEAM_SIZE, out var teamSize))
        {
            return (int)teamSize;
        }

        return -1;
    }
}
