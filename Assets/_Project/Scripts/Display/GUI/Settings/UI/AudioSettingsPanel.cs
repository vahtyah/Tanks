using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private void OnEnable()
    {
        if (sliderSettings.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(sliderSettings.Keys.First().gameObject);
            OptionSelectorEvent.Trigger(null, (sliderSettings.Values.First() as Setting)?.GetSettingName(),
                (sliderSettings.Values.First() as Setting)?.GetSettingDescription());
        }
    }

    private void Start()
    {
        var settings = audioSettingsManager.GetSettings();
        foreach (var setting in settings)
        {
            if (setting is ISliderSetting sliderSetting && setting.settingType == SettingType.Audio)
            {
                var slider = Instantiate(sliderOptionSelector, container);
                slider.Initialize(setting, sliderSetting.GetValue(), sliderSetting.SetValue);
                sliderSettings.Add(slider, sliderSetting);
            }
        }
        
        if (sliderSettings.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(sliderSettings.Keys.First().gameObject);
            OptionSelectorEvent.Trigger(null, (sliderSettings.Values.First() as Setting)?.GetSettingName(),
                (sliderSettings.Values.First() as Setting)?.GetSettingDescription());
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
