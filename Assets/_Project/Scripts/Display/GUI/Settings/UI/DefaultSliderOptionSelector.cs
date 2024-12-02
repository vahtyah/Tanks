using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DefaultSliderOptionSelector : SliderOptionSelector
{
    [Header("Text")] [SerializeField] TMPro.TextMeshProUGUI labelText;
    [Header("Slider")] [SerializeField] UnityEngine.UI.Slider slider;
    
    UnityAction<float> onValueChanged;
    private Setting setting;
    private Button button;
    
    
    public override void SetValue(float value)
    {
        slider.value = value;
    }

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            OptionSelectorEvent.Trigger(null, setting.GetSettingName(), setting.GetSettingDescription());
        });
    }

    private void OnSliderValueChanged(float arg0)
    {
        onValueChanged?.Invoke(arg0);
    }

    public override void Initialize(Setting setting, float value, UnityAction<float> onValueChanged)
    {
        this.setting = setting;
        SetLabelText(setting.GetSettingName());
        SetValue(value);
        this.onValueChanged += onValueChanged;
    }

    private void SetLabelText(string label)
    {
        labelText.text = label;
    }
}