using System;
using UnityEditor;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>, IEventListener<Event>
{
    public int NumberOfPlayers = 2;
    
    
    //State
    public bool IsPaused { get; private set; }
    

    private void OnEnable() { this.StartListening(); }

    private void OnDisable() { this.StopListening(); }

    public void OnEvent(Event e)
    {
        switch (e.EventType)
        {
            case EventType.TogglePause:
                TogglePause();
                break;
            case EventType.GamePause:
                Pause();
                break;
            case EventType.GameUnPause:
                UnPause();
                break;
        }
    }

    private void TogglePause() { Event.Trigger(IsPaused ? EventType.GameUnPause : EventType.GamePause, null); }

    private void Pause()
    {
        GUIManager.Instance.SetPausePanel(true);
        IsPaused = true;
        Time.timeScale = 0;
    }
    
    private void UnPause()
    {
        Time.timeScale = 1;
        GUIManager.Instance.SetPausePanel(false);
        IsPaused = false;
    }
}