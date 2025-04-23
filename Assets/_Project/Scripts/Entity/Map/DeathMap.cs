using UnityEngine;

public class DeathMap : Map
{
    [SerializeField] private int playerCount;
    public int PlayerCount => playerCount;

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
        var spawnArea = SpawnAreaManager.GetRandomSpawnArea();
        return spawnArea.GetSpawnPointByIndex(index);
    }

    public override Transform GetAreaTransform(Team team)
    {
        var spawnArea = SpawnAreaManager.GetRandomSpawnArea();
        return spawnArea.transform;
    }

    public override bool IsQualifiedMap(MapType mapType)
    {
        return mapType == MapType;
    }
}
