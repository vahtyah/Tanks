using System;
using UnityEngine;

public class GUIManager : Singleton<GUIManager>, IEventListener<GameEventType>
{
    [SerializeField] protected GameObject pausePanel;
    // [SerializeField] private GameObject usernamePanel;
    
    protected virtual void Start()
    {
        SetPausePanel(false);
    }
    
    public void SetPausePanel(bool value)
    {
        if(pausePanel == null) return;
        pausePanel.SetActive(value);
    }

    public virtual void SetUsernamePanel(bool value)
    {
        Debug.Log("GUIManager");
        // if(usernamePanel == null) return;
        // usernamePanel.SetActive(value);
    }
    
    public void OnEvent(GameEventType e) {  }

    public virtual void SetUserName(string inputFieldText)
    {
        
    }
    
    public virtual void SetScoreText(int score)
    {
        
    }
    
    //TODO: VCL NGU
    public virtual void SetScoreTextMaskDie(int score) { throw new NotImplementedException(); }
}
