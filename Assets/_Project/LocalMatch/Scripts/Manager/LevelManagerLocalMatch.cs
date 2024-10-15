using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagerLocalMatch : LevelManager
{
    [SerializeField] private List<GameObject> playerPrefabs;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private MultiplayerSplitCameraRig cameraRig;
    [SerializeField] private SplitterCanvasController splitterCanvasController;

    private int numberOfPlayers;
    private List<PlayerCharacter> characters = new();
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

    protected override void PreInitialization()
    {
        cameraRig.InitializeCameras(numberOfPlayers);
        for (int i = 0; i < numberOfPlayers; i++)
        {
            var player = Instantiate(playerPrefabs[i], spawnPoints[i].position, spawnPoints[i].rotation);
            characters.Add(player.GetComponent<PlayerCharacter>());
        }
    }

    protected override void Initialization()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            cameraRig.CameraControllers[i].SetFollowTarget(characters[i].transform);
        }
    }

    public override PlayerCharacter GetPlayer(string playerID)
    {
        return characters.Find(character => character.PlayerID == playerID);
    }

    protected override void CharacterDeath(Character character)
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
}