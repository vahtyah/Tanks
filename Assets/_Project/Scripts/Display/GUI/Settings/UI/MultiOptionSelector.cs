using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class MultiOptionSelector : MonoBehaviour
{
    public abstract void Initialize(string label, List<string> values, int index, UnityAction<int> onSelectionChanged);
    public abstract void SetIndex(int index);
}