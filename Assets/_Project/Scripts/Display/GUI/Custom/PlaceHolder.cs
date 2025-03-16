using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaceHolder : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private TextMeshProUGUI placeholderText;
    
    private void Awake()
    {
        placeholderText = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    public void SetText(string text)
    {
        placeholderText.text = text;
    }
    
    public void Error()
    {
        fillImage.color = Color.red;
        placeholderText.color = Color.red;
    }
}