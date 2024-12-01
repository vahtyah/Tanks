using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsPanel : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private SliderOptionSelector sliderOptionSelector;
    [SerializeField] private Button applyButton;
    
    private GraphicSettingsManager audioSettingsManager;
    private Dictionary<SliderOptionSelector, ISliderSetting> sliderSettings = new Dictionary<SliderOptionSelector, ISliderSetting>();

    private void Awake()
    {
        audioSettingsManager = FindObjectOfType<GraphicSettingsManager>();
        applyButton.onClick.AddListener(() =>
        {
            audioSettingsManager.ApplySettings();
            audioSettingsManager.SaveSettings();
        });
    }

    private void Start()
    {
        var settings = audioSettingsManager.GetSettings();
        foreach (var setting in settings)
        {
            if (setting is ISliderSetting sliderSetting && setting.settingType == SettingType.Audio)
            {
                var slider = Instantiate(sliderOptionSelector, container);
                slider.Initialize(setting.GetSettingName(), sliderSetting.GetValue(), sliderSetting.SetValue);
                sliderSettings.Add(slider, sliderSetting);
            }
        }
    }
    
    public void ResetSettings()
    {
        foreach (var sliderSetting in sliderSettings)
        {
            sliderSetting.Key.SetValue(sliderSetting.Value.GetValue());
        }
    }
}
