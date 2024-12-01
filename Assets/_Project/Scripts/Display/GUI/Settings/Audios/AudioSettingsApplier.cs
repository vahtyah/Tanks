using MoreMountains.Tools;
using UnityEngine;

public class AudioSettingsApplier : GraphicSettingsApplier
{
    private MMSoundManager soundManager;
    
    private void Awake()
    {
        soundManager = MMSoundManager.Instance;
    }
    public override void ApplySettings()
    {
        if (TryGetSetting(out SFXSettings setting))
        {
            soundManager.SetVolumeSfx(setting.GetValue());
        }

        if (TryGetSetting(out MusicSetting musicSetting))
        {
            soundManager.SetVolumeMusic(musicSetting.GetValue());
        }
    }
    
    bool TryGetSetting<T>(out T setting) where T : Setting
    {
        return GraphicSettingsManager.Instance.TryGet(out setting);
    }
}
