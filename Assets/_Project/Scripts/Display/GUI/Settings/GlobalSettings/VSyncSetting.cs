public class VSyncSetting : ToggleGraphicSetting
{
    public override string GetSettingName() => "VSync";
    public override string GetSettingDescription() => "If the VSync is enabled, the game will synchronize the frame rate with the monitor's refresh rate.";
}