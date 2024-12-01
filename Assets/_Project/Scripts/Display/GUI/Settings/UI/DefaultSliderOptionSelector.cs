using System;
using UnityEngine;
using UnityEngine.Events;

public class DefaultSliderOptionSelector : SliderOptionSelector
{
    [Header("Text")] [SerializeField] TMPro.TextMeshProUGUI labelText;
    [Header("Slider")] [SerializeField] UnityEngine.UI.Slider slider;
    
    UnityAction<float> onValueChanged;
    
    public override void SetValue(float value)
    {
        slider.value = value;
    }

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float arg0)
    {
        onValueChanged?.Invoke(arg0);
    }

    public override void Initialize(string label, float value, UnityAction<float> onValueChanged)
    {
        SetLabelText(label);
        SetValue(value);
        this.onValueChanged += onValueChanged;
    }

    private void SetLabelText(string label)
    {
        labelText.text = label;
    }
}