using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject exitWindow;
    
    private void Awake()
    {
        exitButton.onClick.AddListener(OnExitButtonClick);
        
        exitWindow.SetActive(false);
    }

    private void OnExitButtonClick()
    {
        exitWindow.SetActive(true);
    }
}