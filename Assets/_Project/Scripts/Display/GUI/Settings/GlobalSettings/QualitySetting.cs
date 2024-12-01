using UnityEngine;

public class QualitySetting : MultiOptionGraphicSetting<int>
{
    public override void Initialize()
    {
        var names = QualitySettings.names;
        
        for (int i = 0; i < names.Length; i++)
        {
            AddOption(names[i], i);
        }
        
        SetIndex(QualitySettings.GetQualityLevel());
    }

    public override string GetSettingName() => "Quality";

    public override void LoadSetting()
    {
        string key = GetSettingName();
        int defaultValue = QualitySettings.GetQualityLevel();
        int storedValue = SettingsStorage.GetInt(key, defaultValue);
        SetIndex(storedValue);
    }

    public override void SaveSetting()
    {
        string key = GetSettingName();
        int value = GetIndex();
        SettingsStorage.SetInt(key, value);
    }
}
