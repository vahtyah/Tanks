using UnityEngine;
using UnityEngine.Events;

public abstract class SliderOptionSelector : MonoBehaviour
{
    public abstract void Initialize(string label, float value, UnityAction<float> onValueChanged);
    public abstract void SetValue(float value);
}