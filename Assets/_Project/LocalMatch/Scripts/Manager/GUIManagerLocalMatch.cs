using System;
using UnityEngine;

public class GUIManagerLocalMatch : Singleton<GUIManagerLocalMatch>, IEventListener<GameEvent>
{
    [SerializeField] protected GameObject pausePanel;
    [SerializeField] private GameObject selectNumberOfPlayersPanel;
    
    private void Start()
    {
        SetPausePanel(false);
    }

    private void SetPausePanel(bool value)
    {
        if(pausePanel == null) return;
        pausePanel.SetActive(value);
    }
    
    private void SetSelectNumberOfPlayersPanel(bool value)
    {
        if(selectNumberOfPlayersPanel == null) return;
        selectNumberOfPlayersPanel.SetActive(value);
    }

    public void OnEvent(GameEvent e)
    {
        if(e.EventType == GameEventType.TogglePause) return;
        SwitchPanel(e.EventType);
    }

    private void SwitchPanel(GameEventType eventType)
    {
        SetPausePanel(eventType == GameEventType.GamePause);
        SetSelectNumberOfPlayersPanel(eventType == GameEventType.GamePreStart);
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