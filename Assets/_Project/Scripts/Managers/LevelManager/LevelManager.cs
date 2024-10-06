using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class LevelManager : Singleton<LevelManager>, IEventListener<CharacterEvent>, IEventListener<GameEvent>
{
    protected PlayerCharacter winner;

    protected bool isGameOver;

    protected override void Awake() { PreInitialization(); }

    protected virtual void PreInitialization() { }

    private void Start() { Initialization(); }

    protected virtual void Initialization() { }

    private void Update() { CheckForGameOver(); }

    private void CheckForGameOver()
    {
        if (!isGameOver) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameEvent.Trigger(GameEventType.GamePreStart);
            Scene.Load(SceneManager.GetActiveScene().name);
        }
    }

    public abstract PlayerCharacter GetCharacter(string playerID);

    public void OnEvent(CharacterEvent e)
    {
        switch (e.EventType)
        {
            case CharacterEventType.CharacterDeath:
                CharacterDeath(e.Character);
                break;
        }
    }
    protected virtual void CharacterDeath(Character character) { }

    protected IEnumerator IETriggerGameOver()
    {
        yield return new WaitForSecondsRealtime(1);
        isGameOver = true;
        Event.Trigger(EventType.GameOver, winner);
        Debug.Log($"Player {winner.PlayerID} wins!");
    }

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GamePreStart:
                GamePreStart();
                break;
            case GameEventType.GameStart:
                GameStart();
                break;
            case GameEventType.GamePause:
                GamePause();
                break;
            case GameEventType.GameOver:
                GameOver();
                break;
        }
    }

    protected virtual void GamePause() {  }

    protected virtual void GameStart() {  }

    protected virtual void GamePreStart() {  }
    protected virtual void GameOver() {  }

    private void OnEnable()
    {
        this.StartListening<GameEvent>();
        this.StartListening<CharacterEvent>();
    }

    private void OnDisable()
    {
        this.StopListening<GameEvent>();
        this.StopListening<CharacterEvent>();
    }
}