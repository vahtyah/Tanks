using UnityEngine;

public abstract class ToggleGraphicSetting : MultiOptionGraphicSetting<bool>
{
    public bool IsEnabledByDefault = true;
    public override void Initialize()
    {
        AddOption("Off", false);
        AddOption("On", true);

        SelectOption(IsEnabledByDefault);
    }
    
    public bool IsEnabled() => GetSelectedOption();

    public override void LoadSetting()
    {
        bool value = SettingsStorage.GetBool(GetSettingName(), IsEnabled());

        SelectOption(value);
    }

    public override void SaveSetting()
    {
        SettingsStorage.SetBool(GetSettingName(), IsEnabled());
    }
}