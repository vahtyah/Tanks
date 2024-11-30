﻿using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SettingsManager : PersistentSingleton<SettingsManager>
{
    [Log] protected List<GraphicSettingsApplier> GraphicSettingsAppliers { get; set; } = new();
    public abstract List<Setting> GetGraphicSettings();
    
    public abstract T Get<T>() where T : Setting;
    
    public abstract bool TryGet<T>(out T graphicSetting) where T : Setting;
    
    public abstract void LoadSettings();
    
    public abstract void SaveSettings();
    
    public abstract void ApplySettings();

    public void Register(GraphicSettingsApplier graphicSettingsApplier)
    {
        if (graphicSettingsApplier != null && !GraphicSettingsAppliers.Contains(graphicSettingsApplier))
        {
            GraphicSettingsAppliers.Add(graphicSettingsApplier);
        }
    }

    public void Unregister(GraphicSettingsApplier graphicSettingsApplier)
    {
        GraphicSettingsAppliers.Remove(graphicSettingsApplier);
    }
}
[RequireComponent(typeof(GraphicSettingsManager))]
public abstract class Setting : MonoBehaviour, IGraphicSetting
{
    protected SettingsStorage SettingsStorage { get; set; }

    protected virtual void Awake()
    {
        SettingsStorage = GetComponent<SettingsStorage>();
    }

    public abstract void Initialize();
    public abstract string GetSettingName();
    public abstract void LoadSetting();
    public abstract void SaveSetting();
}

public abstract class SettingsStorage : MonoBehaviour
{
    public abstract bool GetBool(string key, bool defaultValue);
    public abstract float GetFloat(string key, float defaultValue);
    public abstract int GetInt(string key, int defaultValue);
    public abstract string GetString(string key, string defaultValue);
    public abstract void SetBool(string key, bool value);
    public abstract void SetFloat(string key, float value);
    public abstract void SetInt(string key, int value);
    public abstract void SetString(string key, string value);
}



public interface IGraphicSetting
{
    void Initialize();
    string GetSettingName();
}

public abstract class GraphicSettingsApplier : MonoBehaviour
{
    protected virtual void Start()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.Register(this);
        }
    }

    // protected virtual void OnEnable()
    // {
    //     Debug.Log("Enabling");
    //     if (SettingsManager.Instance != null)
    //     {
    //         Debug.Log("Registering");
    //         SettingsManager.Instance.Register(this);
    //     }
    // }
    //
    // protected virtual void OnDisable()
    // {
    //     if (SettingsManager.Instance != null)
    //     {
    //         SettingsManager.Instance.Unregister(this);
    //     }
    // }

    public abstract void ApplySettings();
}