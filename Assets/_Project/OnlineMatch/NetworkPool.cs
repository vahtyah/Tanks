using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class NetworkPool : IPunPrefabPool
{
    public readonly Dictionary<string, GameObject> ResourceCache = new Dictionary<string, GameObject>();

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        bool cached = this.ResourceCache.TryGetValue(prefabId, out var res);
        if (!cached)
        {
            res = Resources.Load<GameObject>(prefabId);
            if(res == null)
            {
                Debug.LogError("Failed to load resource: " + prefabId);
                return null;
            }
            this.ResourceCache.Add(prefabId, res);
        }
        
        return Pool.Spawn(res, position, rotation);
    }

    public void Destroy(GameObject gameObject)
    {
        Pool.Despawn(gameObject);
    }
}