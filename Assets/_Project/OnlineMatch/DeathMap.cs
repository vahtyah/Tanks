using UnityEngine;

public class DeathMap : Map
{
    [SerializeField] private int playerCount;
    public int PlayerCount => playerCount;

    public override Vector3 GetRandomSpawnPosition()
    {
        var spawnArea = SpawnAreaManager.GetRandomSpawnArea();
        return spawnArea.GetSpawnPoint();
    }

    public override Vector3 GetSpawnPositionByIndex(int index)
    {
        var spawnArea = SpawnAreaManager.GetRandomSpawnArea();
        return spawnArea.GetSpawnPointByIndex(index);
    }

    public override Transform GetAreaTransform()
    {
        var spawnArea = SpawnAreaManager.GetRandomSpawnArea();
        return spawnArea.transform;
    }

    public override bool IsQualifiedMap(MapType mapType)
    {
        if(mapType == this.MapType)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
