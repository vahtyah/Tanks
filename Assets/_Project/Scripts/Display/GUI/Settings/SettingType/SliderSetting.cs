using UnityEngine;

public interface ISliderSetting
{
    void SetValue(float value);
    float GetValue();
}

public abstract class SliderSetting : Setting, ISliderSetting
{
    [SerializeField] protected float defaultValue = 0.5f;
    
    float value;

    public void SetValue(float value)
    {
        this.value = value;
    }

    public float GetValue() => value;

    public override void Initialize()
    {
        value = defaultValue;
    }

    public override void LoadSetting()
    {
        value = SettingsStorage.GetFloat(GetSettingName(), defaultValue);
        SetValue(value);
    }

    public override void SaveSetting()
    {
        SettingsStorage.SetFloat(GetSettingName(), GetValue());
    }
}