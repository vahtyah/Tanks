using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    private static PoolManager _manager;
    private readonly Queue<GameObject> availableObjects = new Queue<GameObject>();

    public static Pool Register(GameObject prefab, int initialSize = 0)
    {
        EnsureManagerExists();

        var pool = new Pool();
        for (var i = 0; i < initialSize; i++)
        {
            var obj = Object.Instantiate(prefab, _manager.transform);
            obj.SetActive(false);
            pool.availableObjects.Enqueue(obj);
            _manager.TrackObject(obj, pool);
        }

        return _manager.RegisterPool(prefab, pool);
    }

    public static GameObject Get(GameObject prefab)
    {
        EnsureManagerExists();

        var pool = _manager.GetPool(prefab);
        if (pool == null)
            throw new System.Exception("Pool not found");

        if (pool.availableObjects.Count > 0)
        {
            var obj = pool.availableObjects.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        var newObj = Object.Instantiate(prefab, _manager.transform);
        _manager.TrackObject(newObj, pool);
        newObj.SetActive(true);
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
