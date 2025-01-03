﻿using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if (_instance == null) _instance = this as T;
        else Destroy(gameObject);
    }

    protected virtual void OnApplicationQuit() { Destroy(_instance); }

    protected virtual void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}

// public abstract class SerializedSingleton<T> : SerializedMonoBehaviour where T : SerializedSingleton<T>
// {
//     private static T _instance;
//     public static T Instance => _instance;
//
//     protected virtual void Awake()
//     {
//         if (_instance == null) _instance = this as T;
//         else Destroy(gameObject);
//     }
//
//     protected virtual void OnApplicationQuit() { Destroy(_instance); }
//
//     protected virtual void OnDestroy()
//     {
//         if (_instance == this) _instance = null;
//     }
// }

public abstract class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
{
    private static T _instance;
    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if (_instance == null) _instance = this as T;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnApplicationQuit() { Destroy(_instance); }

    protected virtual void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}