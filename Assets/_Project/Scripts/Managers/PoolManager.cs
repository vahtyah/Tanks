using System;
using System.Collections.Generic;
using System.Linq;
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

    public static Pool Register(GameObject prefab, Transform parent = null,  int initialSize = 0)
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

    public GameObject Get()
    {
        EnsureManagerExists();
        if(availableObjects.Count > 0)
        {
            var obj = availableObjects.Dequeue();
            _manager.TrackObject(obj, this);
            obj.SetActive(true);
            return obj;
        }
        
        var newObj = Object.Instantiate(availableObjects.Peek(), parent);
        _manager.TrackObject(newObj, this);
        newObj.SetActive(true);
        return newObj;
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

    public static GameObject Get(GameObject prefab, bool enable = true)
    {
        EnsureManagerExists();

        var pool = _manager.GetPool(prefab);
        if (pool == null)
            throw new System.Exception("Pool not found");

        if (pool.availableObjects.Count > 0)
        {
            var obj = pool.availableObjects.Dequeue();
            _manager.TrackObject(obj, pool);
            obj.SetActive(enable);
            return obj;
        }

        var newObj = Object.Instantiate(prefab, pool.parent);
        _manager.TrackObject(newObj, pool);
        newObj.SetActive(enable);
        return newObj;
    }

    public static void Return(GameObject obj)
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
    
    public List<GameObject> ListPool;

    private void Update()
    {
        ListPool = pools.Keys.ToList();
    }

    public Pool GetPool(GameObject prefab) => pools.GetValueOrDefault(prefab);

    public void TrackObject(GameObject obj, Pool pool) => trackedObjects[obj] = pool;

    public Pool UntrackObject(GameObject obj) =>
        !trackedObjects.Remove(obj, out var pool) ? null : pool;

    public Pool RegisterPool(GameObject prefab, Pool pool)
    {
        pools.TryAdd(prefab, pool);
        return pools[prefab];
    }
}
