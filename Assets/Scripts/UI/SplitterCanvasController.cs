using UnityEngine;

public class SplitterCanvasController : MonoBehaviour
{
    [SerializeField] private GameObject splitterFor2Screens;
    [SerializeField] private GameObject splitterFor3Screens;
    [SerializeField] private GameObject splitterFor4Screens;
    
    private void Start()
    {
        var screenCount = LevelManager.Instance.PlayerNumber;
        switch (screenCount)
        {
            case 2:
                splitterFor2Screens.SetActive(true);
                break;
            case 3:
                splitterFor3Screens.SetActive(true);
                break;
            case 4:
                splitterFor4Screens.SetActive(true);
                break;
        }
    }
}