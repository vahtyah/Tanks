using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphicSettingsManager : SettingsManager
{
    List<Setting> settings = new();
    
    public override List<Setting> GetSettings() => settings;
    public override T Get<T>()
    {
        foreach (var setting in settings)
        {
            if (setting is T targetSetting)
            {
                return targetSetting;
            }
        }

        return default;
    }

    public override bool TryGet<T>(out T graphicSetting)
    {
        foreach (var setting in settings)
        {
            if (setting is T targetSetting)
            {
                graphicSetting = targetSetting;
                return true;
            }
        }

        graphicSetting = default;

        return false;
    }
    
    protected override void Awake()
    {
        base.Awake();
        settings = GetComponents<Setting>().ToList();
        foreach (var setting in settings)
        {
            setting.Initialize();
        }
    }
    
    private void Start()
    {
        LoadSettings();
        ApplySettings();
    }

    public override void LoadSettings()
    {
        foreach (var setting in settings)
        {
            if (setting is { } storeable)
            {
                storeable.LoadSetting();
            }
        }
    }

    public override void SaveSettings()
    {
        foreach (var setting in settings)
        {
            if (setting is { } storeable)
            {
                storeable.SaveSetting();
            }
        }
    }

    public override void ApplySettings()
    {
        foreach (var settingApplier in GraphicSettingsAppliers)
        {
            settingApplier.ApplySettings();
        }
    }
    
    private void Reset()
    {
        if(!TryGetComponent<SettingsStorage>(out _))
        {
            gameObject.AddComponent<GraphicSettingsStorage>();
        }
    }
}
