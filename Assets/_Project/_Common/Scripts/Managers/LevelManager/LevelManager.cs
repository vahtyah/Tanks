using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public abstract class LevelManager : SingletonPunCallbacks<LevelManager>, IEventListener<CharacterEvent>,
    IEventListener<GameEvent>
{
    protected PlayerCharacter winner;

    protected override void Awake()
    {
        base.Awake();
        PreInitialization();
    }

    protected virtual void PreInitialization() { }

    private void Start() { Initialization(); }

    protected virtual void Initialization() { }

    protected virtual void Update() { CheckForGameOver(); }

    private void CheckForGameOver()
    {
        if (GameManager.Instance.currentGameType != GameEventType.GameOver) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameEvent.Trigger(GameEventType.GamePreStart);
            Scene.LoadCurrentScene();
        }
    }

    public abstract PlayerCharacter GetPlayer(string playerID);

    public void OnEvent(CharacterEvent e)
    {
        switch (e.EventType)
        {
            case CharacterEventType.CharacterDeath:
                CharacterDeath(e.Character);
                break;
        }
    }
    
    public virtual PlayerCharacter GetWinner() { return winner; }

    protected virtual void CharacterDeath(Character character) { }

    protected IEnumerator IETriggerGameOver()
    {
        yield return new WaitForSecondsRealtime(1);
        GameEvent.Trigger(GameEventType.GameOver);
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

    protected virtual void GamePause() { }

    protected virtual void GameStart() { }

    protected virtual void GamePreStart() { }
    protected virtual void GameOver() { }

    public override void OnEnable()
    {
        base.OnEnable();
        this.StartListening<GameEvent>();
        this.StartListening<CharacterEvent>();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        this.StopListening<GameEvent>();
        this.StopListening<CharacterEvent>();
    }
}