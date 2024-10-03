using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>, IEventListener<Event>
{
    public int PlayerNumber = 2;
    public List<GameObject> players = new();
    public List<Transform> spawnPoints = new();
    public MultiplayerSplitCameraRig cameraRig;

    protected List<PlayerCharacter> characters = new();
    protected PlayerCharacter winner;
    
    private bool isGameOver;

    protected override void Awake()
    {
        base.Awake();
        PreInitialization();
    }
    
    protected virtual void PreInitialization()
    {
        PlayerNumber = GameManager.Instance.NumberOfPlayers;
        cameraRig.InitializeCameras(PlayerNumber);
        for (int i = 0; i < PlayerNumber; i++)
        {
            var player = Instantiate(players[i], spawnPoints[i].position, spawnPoints[i].rotation);
            characters.Add(player.GetComponent<PlayerCharacter>());
        }
    }

    private void Start()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        for (int i = 0; i < PlayerNumber; i++)
        {
            cameraRig.CameraControllers[i].SetFollowTarget(characters[i].transform);
        }
    }

    private void Update()
    {
        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        if (!isGameOver) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameEvent.Trigger(GameEventType.GamePreStart, null);
            Scene.Load(SceneManager.GetActiveScene().name);
        }
    }

    public PlayerCharacter GetCharacter(string playerID)
    {
        return characters.Find(character => character.PlayerID == playerID);
    }

    public void OnEvent(Event e)
    {
        switch (e.EventType)
        {
            case EventType.PlayerDeath:
                PlayerDead(e.OriginCharacter);
                break;
        }
    }

    protected virtual void PlayerDead(PlayerCharacter character)
    {
        if (character == null) return;
        int alivePlayers =
            characters.Count(c => c.conditionState.CurrentState != CharacterStates.CharacterCondition.Dead);
        if (alivePlayers <= 1)
        {
            winner = characters.FirstOrDefault(c =>
                c.conditionState.CurrentState != CharacterStates.CharacterCondition.Dead);
            StartCoroutine(GameOver());
        }
    }

    protected IEnumerator GameOver()
    {
        yield return new WaitForSecondsRealtime(1);
        isGameOver = true;
        Event.Trigger(EventType.GameOver, winner);
        Debug.Log($"Player {winner.PlayerID} wins!");
    }

    private void OnEnable() { this.StartListening(); }

    private void OnDisable() { this.StopListening(); }
}