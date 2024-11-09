using UnityEngine;

public abstract class LocatableIconComponent : MonoBehaviour
{
    public abstract void SetVisible(bool visibility);
    public abstract void ChangeColor(Color color);
}