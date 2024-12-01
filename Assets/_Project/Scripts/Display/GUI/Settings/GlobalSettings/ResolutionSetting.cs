using UnityEngine;

public class ResolutionSetting : MultiOptionGraphicSetting<Resolution>
{
    public override void Initialize()
    {
        var supportedResolutions = Screen.resolutions;

        foreach (var resolution in supportedResolutions)
        {
            AddOption($"{resolution.width}x{resolution.height}", resolution);
        }

        SelectOption(r => AreEqual(r, Screen.currentResolution), defaultIndex: 0);
    }

    public override string GetSettingName() => "Resolution";

    public override void LoadSetting()
    {
        var currentResolution = Screen.currentResolution;

        Resolution resolution = new()
        {
            width = SettingsStorage.GetInt("Resolution_Width", currentResolution.width),
            height = SettingsStorage.GetInt("Resolution_Height", currentResolution.height)
        };

        SelectOption(r => AreEqual(r, resolution), defaultIndex: 0);
    }

    public override void SaveSetting()
    {
        SettingsStorage.SetInt("Resolution_Width", GetSelectedOption().width);
        SettingsStorage.SetInt("Resolution_Height", GetSelectedOption().height);
    }
    
    bool AreEqual(Resolution a, Resolution b)
    {
        return a.width == b.width && a.height == b.height;
    }
}