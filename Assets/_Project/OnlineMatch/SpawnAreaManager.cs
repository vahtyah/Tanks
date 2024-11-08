using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SpawnAreaManager : MonoBehaviour
{
    private Dictionary<TeamType, SpawnArea> spawnAreas = new();
    
    TeamType teamType = TeamType.None;
    
    private void Awake()
    {
        foreach (Transform child in transform)
        {
            var spawnArea = child.GetComponent<SpawnArea>();
            var teamType = spawnArea.TeamType;
            spawnAreas.Add(teamType, spawnArea);
        }
    }
    
    public SpawnArea GetSpawnAreaByTeam(Team team)
    {
        return spawnAreas[team.TeamType];
    }
    
    public SpawnArea GetRandomSpawnArea()
    {
        var randomIndex = UnityEngine.Random.Range(0, spawnAreas.Count);
        return spawnAreas[(TeamType)randomIndex];
    }
}
