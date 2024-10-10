using Unity.Netcode;
using UnityEngine;

public class NetworkObjectSpawner
{
    public static GameObject Spawn(GameObject prefab, Vector3 position = default)
    {
        GameObject go = Object.Instantiate(prefab);
        go.transform.position = position;
        go.GetComponent<NetworkObject>().Spawn();
        return go;
    }
    public static GameObject Spawn(GameObject prefab, Vector3 position = default, Quaternion rotation = default)
    {
        GameObject go = Object.Instantiate(prefab, position, rotation);
        go.GetComponent<NetworkObject>().Spawn();
        return go;
    }
    
    public static GameObject SpawnWithOwnership(GameObject prefab, ulong clientId, Vector3 position = default, Quaternion rotation = default)
    {
        GameObject go = Object.Instantiate(prefab, position, rotation);
        go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        return go;
    }
}