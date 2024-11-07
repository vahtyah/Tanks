using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAreaManager : MonoBehaviour
{
    private Dictionary<TeamType, SpawnArea> spawnAreas = new();

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            var spawnArea = child.GetComponent<SpawnArea>();
            var teamType = spawnArea.TeamType;
            spawnAreas.Add(teamType, spawnArea);
        }
    }
    
    public SpawnArea GetSpawnArea(TeamType teamType)
    {
        return spawnAreas[teamType];
    }
}
