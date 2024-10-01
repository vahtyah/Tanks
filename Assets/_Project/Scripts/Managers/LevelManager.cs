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

    private List<PlayerCharacter> characters = new();
    private PlayerCharacter winner;
    
    private bool isGameOver;

    protected override void Awake()
    {
        base.Awake();
        PlayerNumber = GameManager.Instance.NumberOfPlayers;
        cameraRig.InitializeCameras(PlayerNumber);
        for (int i = 0; i < PlayerNumber; i++)
        {
            GameObject player = Instantiate(players[i], spawnPoints[i].position, spawnPoints[i].rotation);
            characters.Add(player.GetComponent<PlayerCharacter>());
        }
    }

    private void Start()
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

    private void PlayerDead(Character character)
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

    private IEnumerator GameOver()
    {
        yield return new WaitForSecondsRealtime(1);
        isGameOver = true;
        Event.Trigger(EventType.GameOver, winner);
        Debug.Log($"Player {winner.PlayerID} wins!");
    }

    private void OnEnable() { this.StartListening(); }

    private void OnDisable() { this.StopListening(); }
}