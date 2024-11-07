using System;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private MapType mapType;
    [SerializeField] private TeamCount teamCount;
    public MapType MapType => mapType;
    public TeamCount TeamCount => teamCount;
    private SpawnAreaManager SpawnAreaManager { get; set; }

    private void Awake()
    {
        SpawnAreaManager = GetComponentInChildren<SpawnAreaManager>();
    }
    
    public Vector3 GetRandomSpawnPosition(TeamType teamType)
    {
        var spawnArea = SpawnAreaManager.GetSpawnArea(teamType);
        return spawnArea.GetSpawnPoint();
    }
    
    public Vector3 GetSpawnPositionByIndex(TeamType teamType, int index)
    {
        Debug.Log($"GetSpawnPositionByIndex: {teamType} - {index}");
        var spawnArea = SpawnAreaManager.GetSpawnArea(teamType);
        return spawnArea.GetSpawnPointByIndex(index);
    }
    
    public Vector3 GetAreaPosition(TeamType teamType)
    {
        return GetAreaTransform(teamType).position;
    }
    
    public Transform GetAreaTransform(TeamType teamType)
    {
        var spawnArea = SpawnAreaManager.GetSpawnArea(teamType);
        return spawnArea.transform;
    }
}
