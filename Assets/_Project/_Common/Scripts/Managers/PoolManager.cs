using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Animations;
using Object = UnityEngine.Object;

public interface IPoolable
{
    void OnSpawn();
    void OnDespawn();
}

public class Pool
{
    private static PoolManager _manager;
    private readonly Queue<GameObject> availableObjects = new Queue<GameObject>();
    private Transform parent;

    public static Pool Register(GameObject prefab, Transform parent = null, int initialSize = 0)
    {
        EnsureManagerExists();
        parent ??= _manager.transform;
        var pool = new Pool
        {
            parent = parent
        };
        for (var i = 0; i < initialSize; i++)
        {
            var obj = Object.Instantiate(prefab, parent);
            obj.SetActive(false);
            pool.availableObjects.Enqueue(obj);
            _manager.TrackObject(obj, pool);
        }

        return _manager.RegisterPool(prefab, pool);
    }

    // public void Return(GameObject obj)
    // {
    //     EnsureManagerExists();
    //
    //     if (availableObjects.Contains(obj))
    //         return;
    //
    //     _manager.UntrackObject(obj);
    //     obj.SetActive(false);
    //     availableObjects.Enqueue(obj);
    // }

    public static GameObject Spawn(GameObject prefab, Transform parent = null)
    {
        EnsureManagerExists();

        var pool = _manager.GetPool(prefab) ?? Register(prefab, parent);

        if (pool.availableObjects.Count > 0)
        {
            var obj = pool.availableObjects.Dequeue();
            _manager.TrackObject(obj, pool);
            return obj;
        }

        var newObj = Object.Instantiate(prefab, pool.parent);
        _manager.TrackObject(newObj, pool);
        newObj.SetActive(true);
        return newObj;
    }
    
    public static GameObject Spawn(GameObject prefab, Vector3 position = default, Quaternion rotation = default, bool enable = true)
    {
        EnsureManagerExists();

        var pool = _manager.GetPool(prefab) ?? Register(prefab);

        if (pool.availableObjects.Count > 0)
        {
            var obj = pool.availableObjects.Dequeue();
            _manager.TrackObject(obj, pool);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(enable);
            return obj;
        }

        var newObj = Object.Instantiate(prefab, position, rotation, pool.parent);
        _manager.TrackObject(newObj, pool);
        newObj.SetActive(enable);
        return newObj;
    }
    
    public static GameObject PhotonSpawn(GameObject prefab, Vector3 position = default, Quaternion rotation = default)
    {
        EnsureManagerExists();
    
        var pool = _manager.GetPool(prefab) ?? Register(prefab);

        if (pool.availableObjects.Count > 0)
        {
            var obj = pool.availableObjects.Dequeue();
            _manager.TrackObject(obj, pool);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            return obj;
        }
    
        var newObj = PhotonNetwork.Instantiate(prefab.name, position, rotation);
        _manager.TrackObject(newObj, pool);
        newObj.SetActive(true);
        return newObj;
    }

    public static void Despawn(GameObject obj)
    {
        EnsureManagerExists();

        var pool = _manager.UntrackObject(obj);
        if (pool != null)
        {
            obj.SetActive(false);
            pool.availableObjects.Enqueue(obj);
        }
        else
        {
            Debug.LogWarning("Attempted to return an object that doesn't belong to any pool.");
        }
    }

    private static void EnsureManagerExists()
    {
        if (_manager == null)
            _manager = PoolManager.Instance ?? throw new System.Exception("PoolManager not found");
    }
}

public class PoolManager : Singleton<PoolManager>
{
    private readonly Dictionary<GameObject, Pool> pools = new();
    private readonly Dictionary<GameObject, Pool> trackedObjects = new();
    public List<GameObject> prefabs = new List<GameObject>();

    public Pool GetPool(GameObject prefab) => pools.GetValueOrDefault(prefab);

    public void TrackObject(GameObject obj, Pool pool) => trackedObjects[obj] = pool;

    public Pool UntrackObject(GameObject obj) =>
        !trackedObjects.Remove(obj, out var pool) ? null : pool;

    public Pool RegisterPool(GameObject prefab, Pool pool)
    {
        pools.TryAdd(prefab, pool);
        prefabs.Add(prefab);
        return pools[prefab];
    }
}