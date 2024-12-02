using UnityEngine;

public enum OptionSelectorType
{
    Slider,
    Toggle
}

public struct OptionSelectorEvent
{
    public Sprite Preview;
    public string SettingName;
    public string SettingDescription;
    
    private static OptionSelectorEvent cacheEvent;
    public OptionSelectorEvent(string settingName, string settingDescription, Sprite preview)
    {
        SettingName = settingName;
        SettingDescription = settingDescription;
        Preview = preview;
    }
    
    public static void Trigger(Sprite preview, string settingName, string settingDescription)
    {
        cacheEvent.Preview = preview;
        cacheEvent.SettingName = settingName;
        cacheEvent.SettingDescription = settingDescription;
        EventManger.TriggerEvent(cacheEvent);
    }
}