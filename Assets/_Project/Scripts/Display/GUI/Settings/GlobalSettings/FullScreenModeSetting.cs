using UnityEngine;

[System.Serializable, System.Flags]
public enum FullScreenModeOptions
{
    ExclusiveFullScreen = 1,
    FullScreenWindow = 2,
    MaximizedWindow = 4,
    Windowed = 8,
    Everything = 0b1111
}
[DisallowMultipleComponent]
public class FullScreenModeSetting : MultiOptionGraphicSetting<FullScreenMode>
{
    [SerializeField] private FullScreenModeOptions EnabledModes = FullScreenModeOptions.Everything;
    public FullScreenMode DefaultMode = FullScreenMode.ExclusiveFullScreen;

    public override void Initialize()
    {
        AddOptionIfEnabled(FullScreenMode.Windowed);
        AddOptionIfEnabled(FullScreenMode.FullScreenWindow);
        AddOptionIfEnabled(FullScreenMode.MaximizedWindow);
        AddOptionIfEnabled(FullScreenMode.ExclusiveFullScreen);
        

        if(!IsEnabled(DefaultMode))
        {
            AddOption(GetDisplayName(DefaultMode), DefaultMode);
        }

        SelectOption(DefaultMode);
    }
    
    
    void AddOptionIfEnabled(FullScreenMode option)
    {
        if (IsEnabled(option))
        {
            AddOption(GetDisplayName(option), option);
        }
    }
    
    bool IsEnabled(FullScreenMode fullScreenMode)
    {
        return fullScreenMode switch
        {
            FullScreenMode.ExclusiveFullScreen => EnabledModes.HasFlag(FullScreenModeOptions.ExclusiveFullScreen),
            FullScreenMode.FullScreenWindow => EnabledModes.HasFlag(FullScreenModeOptions.FullScreenWindow),
            FullScreenMode.MaximizedWindow => EnabledModes.HasFlag(FullScreenModeOptions.MaximizedWindow),
            FullScreenMode.Windowed => EnabledModes.HasFlag(FullScreenModeOptions.Windowed),
            _ => false,
        };
    }
    
    string GetDisplayName(FullScreenMode fullScreenMode) => fullScreenMode switch
    {
        FullScreenMode.ExclusiveFullScreen => "Exclusive Fullscreen",
        FullScreenMode.FullScreenWindow => "Full Screen Window",
        FullScreenMode.MaximizedWindow => "Maximized Window",
        FullScreenMode.Windowed => "Window",
        _ => throw new System.NotImplementedException(),
    };

    public override string GetSettingName() => "Full Screen Mode";
    public override string GetSettingDescription() => "Choose the preferred window mode.";

    public override void SaveSetting()
    {
        SettingsStorage.SetInt("Full Screen Mode", (int)GetSelectedOption());
    }
    
    public override void LoadSetting()
    {
        int value = SettingsStorage.GetInt("Full Screen Mode", (int)DefaultMode);
        SelectOption((FullScreenMode)value);
    }
}