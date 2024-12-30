using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    [SerializeField] private TeamType teamType;
    public TeamType TeamType => teamType;

    
    private ISpawnPoint spawner; 

    private void Awake()
    {
        var spawnPoints = new List<Vector3>();
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child.position);
        }

        spawner = new AreaSpawnPoint(spawnPoints);
    }
    
    public Vector3 GetSpawnPoint()
    {
        return spawner.NextRandomSpawnPoint();
    }
    
    public Vector3 GetSpawnPointByIndex(int index)
    {
        return spawner.NextSpawnPointByIndex(index);
    }
}
