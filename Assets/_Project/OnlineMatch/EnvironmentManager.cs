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
        CurrentMap = LoadTeamMap(mapType);
    }
    

    private Map GetMap(MapType mapType)
    {
        return maps.Find(map => map.IsQualifiedMap(mapType));
    }


    private Map LoadTeamMap(MapType mapType)
    {
        var map = GetMap(mapType);
        if (map == null)
        {
            Debug.LogError($"Map not found for map type: {mapType}");
            return null;
        }
        return Instantiate(map,transform);
    }
}
