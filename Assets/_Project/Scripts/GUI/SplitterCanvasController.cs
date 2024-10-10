using UnityEngine;

public class SplitterCanvasController : MonoBehaviour
{
    [SerializeField] private GameObject splitterFor2Screens;
    [SerializeField] private GameObject splitterFor3Screens;
    [SerializeField] private GameObject splitterFor4Screens;
    
    public void SetSplitterCanvasActive(int numberOfPlayers)
    {
        splitterFor2Screens.SetActive(numberOfPlayers == 2);
        splitterFor3Screens.SetActive(numberOfPlayers == 3);
        splitterFor4Screens.SetActive(numberOfPlayers == 4);
    }
}