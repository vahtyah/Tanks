using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagerLocalMatch : Singleton<LevelManagerLocalMatch>, IEventListener<CharacterEvent>,
    IEventListener<GameEvent>
{
    [SerializeField] private List<GameObject> playerPrefabs;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private MultiplayerSplitCameraRig cameraRig;
    [SerializeField] private SplitterCanvasController splitterCanvasController;

    private int numberOfPlayers;
    private List<PlayerCharacter> characters = new();

    private PlayerCharacter winner;

    protected bool isGameOver;
    private bool isSetup;

    public int NumberOfPlayers
    {
        get => numberOfPlayers;
        set
        {
            if (value is < 1 or > 4)
                throw new System.ArgumentOutOfRangeException($"Number of players must be between 1 and 4");
            numberOfPlayers = value;
        }
    }

    private void Start() { GameEvent.Trigger(GameEventType.GamePreStart); }

    private void Update()
    {
        CheckForGameOver(); 
    }

    private void CheckForGameOver()
    {
        if (!isGameOver) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameEvent.Trigger(GameEventType.GamePreStart);
            Scene.LoadCurrentScene();
        }
    }

    private void PreInitialization()
    {
        cameraRig.InitializeCameras(numberOfPlayers);
        for (int i = 0; i < numberOfPlayers; i++)
        {
            var player = Instantiate(playerPrefabs[i], spawnPoints[i].position, spawnPoints[i].rotation);
            characters.Add(player.GetComponent<PlayerCharacter>());
        }
    }

    private void Initialization()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            cameraRig.CameraControllers[i].SetFollowTarget(characters[i].transform);
        }
    }

    public PlayerCharacter GetPlayer(string playerID)
    {
        Debug.Log(characters.Count);
        return characters.Find(character => character.PlayerID == playerID);
    }

    private void CharacterDeath(Character character)
    {
        character = character as PlayerCharacter;
        if (character == null) return;
        var alivePlayers =
            characters.Count(c => c.conditionState.CurrentState != CharacterStates.CharacterCondition.Dead);
        if (alivePlayers <= 1)
        {
            winner = characters.FirstOrDefault(c =>
                c.conditionState.CurrentState != CharacterStates.CharacterCondition.Dead);
            StartCoroutine(IETriggerGameOver());
        }
    }

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GameStart:
                if (isSetup) return;
                PreInitialization();
                Initialization();
                splitterCanvasController.SetSplitterCanvasActive(NumberOfPlayers);
                isSetup = true;
                break;
        }
    }

    public void OnEvent(CharacterEvent e)
    {
        switch (e.EventType)
        {
            case CharacterEventType.CharacterDeath:
                CharacterDeath(e.Character);
                break;
        }
    }

    private IEnumerator IETriggerGameOver()
    {
        yield return new WaitForSecondsRealtime(1);
        isGameOver = true;
        GameEvent.Trigger(GameEventType.GameOver);
    }
    
    public PlayerCharacter GetWinner()
    {
        return winner;
    }

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