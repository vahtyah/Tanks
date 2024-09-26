using UnityEngine;

public class Splitter3Screen : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform1;
    [SerializeField] private RectTransform rectTransform2;
    
    private void Start()
    {
        var width = Screen.width;
        var halfWidth = width / 3;
        rectTransform1.anchoredPosition = new Vector2(halfWidth, rectTransform1.anchoredPosition.y);
        rectTransform2.anchoredPosition = new Vector2(halfWidth * 2, rectTransform2.anchoredPosition.y);
    }
}