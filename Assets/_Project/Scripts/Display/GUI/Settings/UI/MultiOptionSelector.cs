using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class MultiOptionSelector : MonoBehaviour
{
    public abstract void Initialize(Setting setting, List<string> values, int index, UnityAction<int> onSelectionChanged);
    public abstract void SetIndex(int index);
}