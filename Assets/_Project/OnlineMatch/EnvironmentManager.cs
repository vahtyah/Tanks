using System.Collections.Generic;
using Photon.Pun;
using Sirenix.Serialization;
using UnityEngine;

public enum MapType
{
    Default
}

public class EnvironmentManager : Singleton<EnvironmentManager>
{
    [SerializeField] private MapType mapType;
    [SerializeField] private List<Map> maps;

    public Map CurrentMap { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        CurrentMap = LoadMap(MapType.Default, GetTeamCount());
    }
    
    private TeamCount GetTeamCount()
    {
        var teamSize = PhotonNetwork.CurrentRoom.GetTeamSize();
        return teamSize switch
        {
            2 => TeamCount.TwoTeams,
            3 => TeamCount.ThreeTeams,
            4 => TeamCount.FourTeams,
            _ => TeamCount.TwoTeams
        };
    }

    private Map GetMap(MapType mapType , TeamCount teamCount)
    {
        return maps.Find(map => map.MapType == mapType && map.TeamCount == teamCount);
    }

    private Map LoadMap(MapType mapType, TeamCount teamCount)
    {
        var map = GetMap(mapType, teamCount);
        if (map == null)
        {
            Debug.LogError($"Map not found for team count: {teamCount} and map type: {mapType}");
            return null;
        }
        return Instantiate(map,transform);
    }
}
