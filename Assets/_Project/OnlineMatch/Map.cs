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

    public abstract Vector3 GetRandomSpawnPosition();

    public abstract Vector3 GetSpawnPositionByIndex(int index);

    public abstract Transform GetAreaTransform();
    
    public abstract bool IsQualifiedMap(MapType mapType);
}
