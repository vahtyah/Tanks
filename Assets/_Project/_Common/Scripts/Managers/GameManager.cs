using System;
using System.Collections;
using UnityEngine;

public sealed class GameManager : PersistentSingleton<GameManager>, IEventListener<GameEvent>
{
    public GameEventType currentGameType;
    private GameEventType previousGameType { get; set; }

    private void Start() { GameEvent.Trigger(GameEventType.GameMainMenu); }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) &&
            currentGameType is GameEventType.GameStart or GameEventType.GamePause)
            GameEvent.Trigger(GameEventType.TogglePause);
    }

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

    private void GameMainMenu()
    {
        Cursor.visible = true;
        Scene.Load(Scene.SceneName.MainMenu);
    }

    private void GamePreStart() { Cursor.visible = true; }

    private void GameStart()
    {
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Confined;
    }

    private void GamePause() { Cursor.visible = true; }

    private void TogglePause()
    {
        GameEvent.Trigger(currentGameType == GameEventType.GamePause ? previousGameType : GameEventType.GamePause);
    }

    private void OnEnable() { this.StartListening<GameEvent>(); }

    private void OnDisable() { this.StopListening<GameEvent>(); }
}