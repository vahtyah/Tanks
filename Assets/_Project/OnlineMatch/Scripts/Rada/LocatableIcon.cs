using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class LocatableIcon : LocatableIconComponent
{
    protected CanvasGroup CanvasGroup { get; set; }
    private Image image;

    protected virtual void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
    }

    public override void SetVisible(bool visibility)
    {
        CanvasGroup.alpha = visibility ? 1.0f : 0.0f;
    }

    public override void ChangeColor(Color color)
    {
        image.color = color;
    }
}