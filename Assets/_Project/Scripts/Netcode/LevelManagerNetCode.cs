using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagerNetCode : SingletonNetwork<LevelManagerNetCode>, IEventListener<CharacterEvent>
{
    public bool isGameOver;
    public List<Transform> spawnPoints;
    public ISpawnPoint spawner;

    public Dictionary<ulong, PlayerCharacter> playerCharacters = new Dictionary<ulong, PlayerCharacter>();
    
    protected override void Awake()
    {
        base.Awake();
        spawner = new LinearSpawnPoint(spawnPoints);
    }

    private void Start()
    {
        GameEvent.Trigger(GameEventType.GamePreStart);
        NetworkManager.OnClientConnectedCallback += (clientId) =>
        {
            if (IsServer)
            {
                var player = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerCharacter>();
                playerCharacters.TryAdd(clientId, player);
            }
        };
        NetworkManager.OnClientDisconnectCallback += (clientId) =>
        {
            if (IsServer)
                playerCharacters.Remove(clientId);
        };
    }

    private void Update()
    {
        if (!isGameOver) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NetworkManager.Singleton.Shutdown();
            GameEvent.Trigger(GameEventType.GamePreStart);
            Scene.Load(SceneManager.GetActiveScene().name);
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
    
    private void CharacterDeath(Character eCharacter)
    {
        isGameOver = true;
        GameEvent.Trigger(GameEventType.GameOver);
    }

    private void OnEnable() { this.StartListening(); }

    private void OnDisable() { this.StopListening(); }

    public void RemovePlayer(ulong clientId) { playerCharacters.Remove(clientId); }
}