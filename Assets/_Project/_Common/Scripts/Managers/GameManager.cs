using System;
using System.Collections;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public enum GameMode
{
    Deathmatch,
    TeamDeathmatch,
    CaptureTheFlag
}

public sealed class GameManager : PersistentSingleton<GameManager>, IEventListener<GameEvent>
{
    [Log] public CharacterData SelectedCharacter { get; set; }
    [Log] public GameMode GameMode { get; private set; }

    [Log] public GameEventType CurrentGameType { get; private set; }

    private GameEventType previousGameType { get; set; }

    private void Start()
    {
        Application.targetFrameRate = 60;
        
        GameEvent.Trigger(GameEventType.GameMainMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) &&
            CurrentGameType is GameEventType.GameRunning or GameEventType.GamePause)
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
        previousGameType = CurrentGameType;
        CurrentGameType = e.EventType;
    }

    private void GameMainMenu()
    {
        Cursor.visible = true;
    }

    private void GamePreStart()
    {
        Cursor.visible = true;
    }

    private void GameStart()
    {
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Confined;
    }

    private void GamePause()
    {
        Cursor.visible = true;
    }

    private void TogglePause()
    {
        GameEvent.Trigger(CurrentGameType == GameEventType.GamePause ? previousGameType : GameEventType.GamePause);
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