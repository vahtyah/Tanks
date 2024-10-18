using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyMainMenuPanel : MonoBehaviourPunCallbacks
{
    //DEBUG
    [SerializeField] private GameObject debugPanel;
    //Loading
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI loadingText;
    
    [SerializeField] private GameObject multiplayerPanel;

    //Button
    [SerializeField] private Button createRoomButton;

    //Room
    private const string gameVersion = "1";
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private Transform playerListEntry;
    [SerializeField] private GameObject playerEntryPrefab;

    //List Room
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private Transform roomListEntry;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leftRoomButton;

    //Player
    [SerializeField] private TMP_InputField playerNameInput;
    string playerName = "Player";

    //Error
    [SerializeField] private GameObject gameFullWindow;

    private Dictionary<int, PlayerEntry> playerListEntries;
    private Dictionary<string, RoomEntry> roomListEntries;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        playerName += UnityEngine.Random.Range(0, 1000);
        playerNameInput.text = playerName;
        PhotonNetwork.NickName = playerName;
        playerListEntries = new Dictionary<int, PlayerEntry>();
        roomListEntries = new Dictionary<string, RoomEntry>();
        startGameButton.gameObject.SetActive(false);
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
        leftRoomButton.onClick.AddListener(OnLeftRoomButtonClicked);
        playerNameInput.onValueChanged.AddListener((value) => { PhotonNetwork.NickName = value; });
    }

    private void OnJoinRoomButtonClicked()
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        PhotonNetwork.JoinRoom("Room");
    }

    private void Start()
    {
        createRoomButton.interactable = false;
        OnLoginButtonClicked();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            debugPanel.SetActive(!debugPanel.activeSelf);
        }
        if (loadingPanel.activeSelf)
            loadingText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    private void OnLoginButtonClicked()
    {
        // Do connection loop:
        StartCoroutine(Reconnection());
    }

    // This will automatically connect the client to the server every 2 seconds if not connected:
    IEnumerator Reconnection()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            if (!PhotonNetwork.IsConnected || PhotonNetwork.NetworkClientState == ClientState.ConnectingToMasterServer)
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }
    }

    private void OnStartGameButtonClicked()
    {
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     PhotonNetwork.CurrentRoom.IsOpen = false;
        //     PhotonNetwork.CurrentRoom.IsVisible = false;
        //     PhotonNetwork.LoadLevel(Scene.SceneName.OnlineMatch.ToString());
        // }
        // else
        // {
        //     var props = new Hashtable() { { GlobalString.PLAYER_READY, true } };
        //     PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        // }
    }

    public void OnCreateRoomButtonClicked()
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        roomPanel.SetActive(true);
        var roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            IsOpen = true,
            IsVisible = true
        };

        PhotonNetwork.CreateRoom(PhotonNetwork.NickName + "'s room", roomOptions);
    }

    private void OnLeftRoomButtonClicked()
    {
        if (!PhotonNetwork.InRoom) return;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnConnectedToMaster() { PhotonNetwork.JoinLobby(); }

    public override void OnJoinedLobby()
    {
        loadingPanel.SetActive(false);
        multiplayerPanel.SetActive(true);
        createRoomButton.interactable = true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if (room.RemovedFromList)
            {
                if (roomListEntries.TryGetValue(room.Name, out var roomObj))
                {
                    Pool.Despawn(roomObj.gameObject);
                    roomListEntries.Remove(room.Name);
                }
            }
            else
            {
                if (roomListEntries.TryGetValue(room.Name, out var roomEntry))
                {
                    roomEntry.UpdateInfo(room);
                }
                else
                {
                    var roomObj = Pool.Spawn(roomPrefab, roomListEntry);
                    roomObj.GetComponent<RoomEntry>().Initialize(room);
                    roomListEntries.Add(room.Name, roomObj.GetComponent<RoomEntry>());
                }
            }
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        gameFullWindow.SetActive(true);
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
        var playerEntry =
            PlayerEntry.Create(playerEntryPrefab, newPlayer.NickName, newPlayer.ActorNumber, playerListEntry);
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

    public override void OnJoinedRoom()
    {
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

    public override void OnLeftRoom()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { GlobalString.PLAYER_READY, false } });
        foreach (var playerEntry in playerListEntries)
        {
            Destroy(playerEntry.Value.gameObject);
        }

        playerListEntries.Clear();
        roomPanel.SetActive(false);
    }

    private bool CheckAllPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient) return false;
        if (PhotonNetwork.PlayerList.Length < 2) return false;

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

    private void LocalPlayerPropertiesUpdated()
    {
        if (CheckAllPlayersReady())
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    var props = new Hashtable() { { GlobalString.PLAYER_READY, false } };
                    player.SetCustomProperties(props);
                }
            }

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(Scene.SceneName.OnlineMatch.ToString());
        }
    }
}