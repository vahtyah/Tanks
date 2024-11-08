using UnityEngine;

public abstract class Map : MonoBehaviour
{
    [SerializeField] private MapType mapType;
    public MapType MapType => mapType;

    protected SpawnAreaManager SpawnAreaManager { get; private set; }

    private void Awake()
    {
        SpawnAreaManager = GetComponentInChildren<SpawnAreaManager>();
    }

    public abstract Vector3 GetRandomSpawnPosition(Team team);

    public abstract Vector3 GetSpawnPositionByIndex(Team team, int index);

    public abstract Transform GetAreaTransform(Team team);

    public abstract bool IsQualifiedMap(MapType mapType);
}