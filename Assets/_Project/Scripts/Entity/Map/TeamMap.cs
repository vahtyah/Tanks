using System;
using Photon.Pun;
using UnityEngine;

public class TeamMap : Map
{
    [SerializeField] private TeamCount teamCount;
    public TeamCount TeamCount => teamCount;

    public override bool IsQualifiedMap(MapType mapType)
    {
        return MapType == mapType && teamCount == GetTeamCount();
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
    
    public override Vector3 GetRandomSpawnPosition(Team team)
    {
        var spawnArea = SpawnAreaManager.GetSpawnAreaByTeam(team);
        return spawnArea.GetSpawnPoint();
    }

    public override Vector3 GetRandomSpawnPosition()
    {
        var spawnArea = SpawnAreaManager.GetRandomSpawnArea();
        return spawnArea.GetSpawnPoint();
    }


    public override Vector3 GetSpawnPositionByIndex(Team team, int index)
    {
        var spawnArea = SpawnAreaManager.GetSpawnAreaByTeam(team);
        return spawnArea.GetSpawnPointByIndex(index);
    }
    
    public override Transform GetAreaTransform(Team team)
    {
        var spawnArea = SpawnAreaManager.GetSpawnAreaByTeam(team);
        return spawnArea.transform;
    }
}
