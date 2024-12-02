using UnityEngine;

public class MusicSetting : SliderSetting
{
    public override string GetSettingName() => "Music";
    public override string GetSettingDescription()
    {
        return "Tweak the volume of music and soundtracks.";
    }
}
