using System;
using UnityEngine;

public class GUIManagerLocalMatch : Singleton<GUIManagerLocalMatch>, IEventListener<GameEvent>
{
    [SerializeField] protected GameObject pausePanel;
    
    protected virtual void Start()
    {
        SetPausePanel(false);
    }

    private void SetPausePanel(bool value)
    {
        if(pausePanel == null) return;
        pausePanel.SetActive(value);
    }

    public virtual void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GameStart:
                SetPausePanel(false);
                break;
            case GameEventType.GamePause:
                SetPausePanel(true);
                break;
        }
    }
    
    private void OnEnable()
    {
        this.StartListening();
    }
    
    private void OnDisable()
    {
        this.StopListening();
    }
}
