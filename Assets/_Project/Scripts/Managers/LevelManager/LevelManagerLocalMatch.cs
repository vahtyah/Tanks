using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManagerLocalMatch : Singleton<LevelManagerLocalMatch>, IEventListener<CharacterEvent>
{
    [SerializeField] private List<GameObject> playerPrefabs;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private MultiplayerSplitCameraRig cameraRig;
    
    private int numberOfPlayers;
    private List<PlayerCharacter> characters = new();

    private PlayerCharacter winner;

    protected bool isGameOver;
    
    public int NumberOfPlayers
    {
        get => numberOfPlayers;
        private set
        {
            if (value is < 1 or > 4) throw new System.ArgumentOutOfRangeException($"Number of players must be between 1 and 4");
            numberOfPlayers = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        PreInitialization();
    }

    private void Start()
    {
        Initialization();
    }

    private void PreInitialization()
    {
        NumberOfPlayers = GameManager.Instance.NumberOfPlayers;
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
        GameEvent.Trigger(GameEventType.GameStart);
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
    
    private void OnEnable()
    {
        this.StartListening();
    }
    
    private void OnDisable()
    {
        this.StopListening();
    }
}