using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMainMenuPanel : MonoBehaviourPunCallbacks
{
    //Room
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private Transform playerListEntry;
    [SerializeField] private GameObject playerEntryPrefab;
    [SerializeField] private Button startGameButton;
    string playerName = "Player";
    
    private Dictionary<int, PlayerEntry> playerListEntries;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        playerName += UnityEngine.Random.Range(0, 1000);
        
        playerListEntries = new Dictionary<int, PlayerEntry>();
    }

    private void Start() { OnLoginButtonClicked(); }

    public void OnLoginButtonClicked()
    {
        PhotonNetwork.NickName = playerName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
        PhotonNetwork.JoinOrCreateRoom("Room", new Photon.Realtime.RoomOptions { MaxPlayers = 4 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        roomPanel.SetActive(true);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var playerEntry =
                PlayerEntry.Create(playerEntryPrefab, player.NickName, player.ActorNumber, playerListEntry);

            if (player.CustomProperties.TryGetValue(GlobalString.PLAYER_READY, out var isReady))
            {
                playerEntry.SetReadyButton((bool)isReady);
            }
            else
            {
                var props = new Hashtable() { { GlobalString.PLAYER_READY, false } };
                player.SetCustomProperties(props);
            }
            
            playerListEntries.Add(player.ActorNumber, playerEntry);
        }

        startGameButton.gameObject.SetActive(CheckAllPlayersReady());
    }

    private bool CheckAllPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient) return false;
        if(PhotonNetwork.PlayerList.Length < 2) return false;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(GlobalString.PLAYER_READY, out var isReady))
            {
                if (!(bool)isReady) return false;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out var playerEntry))
        {
            if (changedProps.TryGetValue(GlobalString.PLAYER_READY, out var isReady))
            {
                playerEntry.SetReadyButton((bool)isReady);
            }
        }

        LocalPlayerPropertiesUpdated();
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        var playerEntry = PlayerEntry.Create(playerEntryPrefab, newPlayer.NickName, newPlayer.ActorNumber, playerListEntry);
        playerListEntries.Add(newPlayer.ActorNumber, playerEntry);
        LocalPlayerPropertiesUpdated();
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (playerListEntries.Remove(otherPlayer.ActorNumber, out var playerEntry))
        {
            Destroy(playerEntry.gameObject);
        }
        
        LocalPlayerPropertiesUpdated();
    }

    private void LocalPlayerPropertiesUpdated() { startGameButton.gameObject.SetActive(CheckAllPlayersReady()); }
}