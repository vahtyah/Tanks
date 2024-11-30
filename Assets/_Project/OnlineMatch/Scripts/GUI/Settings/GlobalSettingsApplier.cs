using UnityEngine;

public class GlobalSettingsApplier : GraphicSettingsApplier
{
    public override void ApplySettings()
    {
        ApplyScreenSettings();
        ApplyQuality();
        ApplyVSync();
    }

    private void ApplyScreenSettings()
    {
        var resolution = Screen.currentResolution;
        var fullScreenMode = Screen.fullScreenMode;
        
        if (TryGetSetting<ResolutionSetting>(out var resolutionSetting))
        {
            resolution = resolutionSetting.GetSelectedOption();
        }
        
        if (TryGetSetting<FullScreenModeSetting>(out var fullScreenModeSetting))
        {
            fullScreenMode = fullScreenModeSetting.GetSelectedOption();
        }
        
        if (resolutionSetting != null || fullScreenModeSetting != null)
        {
            Screen.SetResolution(resolution.width, resolution.height, fullScreenMode);
        }
    }
    
    void ApplyVSync()
    {
        if (TryGetSetting<VSyncSetting>(out var setting))
        {
            bool isEnabled = setting.GetSelectedOption();

            QualitySettings.vSyncCount = isEnabled ? 1 : 0;
        }
    }

    void ApplyQuality()
    {
        if (TryGetSetting<QualitySetting>(out var setting))
        {
            int qualityLevel = setting.GetSelectedOption();

            QualitySettings.SetQualityLevel(qualityLevel);
        }
    }
    
    bool TryGetSetting<T>(out T setting) where T : Setting
    {
        return GraphicSettingsManager.Instance.TryGet(out setting);
    }
}
