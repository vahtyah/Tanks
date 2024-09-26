using UnityEngine;

public class GUIManager : Singleton<GUIManager>
{
    [SerializeField] private GameObject pausePanel;
    
    private void Start()
    {
        SetPausePanel(false);
    }
    
    public void SetPausePanel(bool value)
    {
        if(pausePanel == null) return;
        pausePanel.SetActive(value);
    }
}
