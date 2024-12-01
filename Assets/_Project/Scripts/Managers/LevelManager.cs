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
    [SerializeField] private List<MultiKill> multiKills;
    public GameMode GameMode { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        PreInitialize();
    }

    protected virtual void PreInitialize() { }

    private void Start() { Initialize(); }

    protected virtual void Initialize() { }

    protected virtual void Update() { CheckForGameOver(); }

    protected virtual void CheckForGameOver()
    {
        if (GameManager.Instance.CurrentGameType != GameEventType.GameOver) return;
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
                HandleCharacterDeath(e.Character);
                break;
        }
    }

    public virtual PlayerCharacter GetWinner() { return winner; }

    protected virtual void HandleCharacterDeath(Character character) { }

    protected IEnumerator TriggerGameOverAfterDelay()
    {
        yield return new WaitForSecondsRealtime(1);
        GameEvent.Trigger(GameEventType.GameOver);
    }
    
    public List<MultiKill> MultiKills => multiKills;

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
            case GameEventType.GameRunning:
                GameRunning();
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
    protected virtual void GameRunning() { }

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