using System;
using System.Collections;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>, IEventListener<GameEvent>
{
    private int numberOfPlayers = 2;
    public GameEventType currentGameType;
    private GameEventType previousGameType { get; set; }
    
    public int NumberOfPlayers
    {
        get => numberOfPlayers;
        set
        {
            if(value is < 1 or > 4) throw new ArgumentOutOfRangeException($"Number of players must be between 1 and 4");
            numberOfPlayers = value;
        }
    }

    protected void Start() { GameEvent.Trigger(GameEventType.GameMainMenu); }

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GameMainMenu:
                GameMainMenu();
                break;
            case GameEventType.GamePreStart:
                GamePreStart();
                break;
            case GameEventType.GameStart:
                GameStart();
                break;
            case GameEventType.GamePause:
                GamePause();
                break;
            case GameEventType.TogglePause:
                TogglePause();
                break;
        }

        if (e.EventType == GameEventType.TogglePause) return;
        previousGameType = currentGameType;
        currentGameType = e.EventType;
    }

    private void GameMainMenu() { Cursor.visible = true; }

    private void GamePreStart() { Cursor.visible = true; }

    private void GameStart()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    protected virtual void GamePause() { Cursor.visible = true; }

    protected IEnumerator GameOver(Character character)
    {
        yield return new WaitForSecondsRealtime(1);
        GameEvent.Trigger(GameEventType.GameOver);
    }

    private void TogglePause()
    {
        GameEvent.Trigger(currentGameType == GameEventType.GamePause ? previousGameType : GameEventType.GamePause);
    }

    private void OnEnable()
    {
        this.StartListening<GameEvent>();
    }

    private void OnDisable()
    {
        this.StopListening<GameEvent>();
    }
}