using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ResolutionSetting : MultiOptionGraphicSetting<Resolution>
{
    private readonly (int width, int height)[] standardResolutions = new[]
    {
        (1280, 720),   // HD
        (1366, 768),   // HD+
        (1600, 900),   // HD+
        (1920, 1080),  // Full HD
        (2560, 1440),  // QHD
        (3840, 2160)   // 4K UHD
    };

    public override void Initialize()
    {
        var supportedResolutions = Screen.resolutions;
        var filteredResolutions = new List<Resolution>();

        foreach (var resolution in supportedResolutions)
        {
            if (IsStandardResolution(resolution))
            {
                if (!filteredResolutions.Any(r => r.width == resolution.width && r.height == resolution.height))
                {
                    filteredResolutions.Add(resolution);
                    AddOption($"{resolution.width}x{resolution.height}", resolution);
                }
            }
        }

        if (filteredResolutions.Count == 0)
        {
            AddOption($"{Screen.currentResolution.width}x{Screen.currentResolution.height}", Screen.currentResolution);
        }

        SelectOption(r => AreEqual(r, Screen.currentResolution), defaultIndex: 0);
    }

    private bool IsStandardResolution(Resolution resolution)
    {
        foreach (var std in standardResolutions)
        {
            if (resolution.width == std.width && resolution.height == std.height)
            {
                return true;
            }
        }
        return false;
    }

    public override string GetSettingName() => "Resolution";
    public override string GetSettingDescription() => "Choose the resolution of the game.";

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