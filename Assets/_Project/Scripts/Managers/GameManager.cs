using System;
using UnityEditor;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>, IEventListener<Event>, IEventListener<GameEvent>
{
    public int NumberOfPlayers = 2;

    public int score = 0;

    public GameEventType CurrentGameType;

    //State
    public bool IsPaused { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        GameEvent.Trigger(GameEventType.GameMainMenu, null);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void OnEvent(Event e)
    {
        switch (e.EventType)
        {
            case EventType.PlayerDeath:
                if (e.OriginCharacter is null)
                {
                    score++;
                    GUIManager.Instance.SetScoreText(score);
                }
                else
                {
                    GUIManager.Instance.SetScoreTextMaskDie(score);
                    DatabaseManager.Instance.WriteNewUser(score);
                }
                break;
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

    public void OnEvent(GameEvent e)
    {
        SetCurrentGameType(e.EventType);
        switch (e.EventType)
        {
            case GameEventType.GameMainMenu:
                Cursor.visible = true;
                break;
            case GameEventType.GamePreStart:
                score = 0;
                Cursor.visible = true;
                Time.timeScale = 0;
                break;
            case GameEventType.GameStart:
                Cursor.visible = false;
                GUIManager.Instance.SetUsernamePanel(false);
                Time.timeScale = 1;
                break;
        }
    }

    private void SetCurrentGameType(GameEventType gameType)
    {
        Debug.Log("Current " + CurrentGameType);
        CurrentGameType = gameType;
    }

    private void TogglePause() { Event.Trigger(IsPaused ? EventType.GameUnPause : EventType.GamePause, null); }

    private void Pause()
    {
        GUIManager.Instance.SetPausePanel(true);
        IsPaused = true;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    private void UnPause()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        GUIManager.Instance.SetPausePanel(false);
        IsPaused = false;
        GameEvent.Trigger(CurrentGameType, null);
    }

    private void OnEnable()
    {
        this.StartListening<Event>();
        this.StartListening<GameEvent>();
    }

    private void OnDisable()
    {
        this.StopListening<Event>();
        this.StopListening<GameEvent>();
    }
}