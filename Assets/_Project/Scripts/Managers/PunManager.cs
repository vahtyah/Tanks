using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class PunManager : SingletonPunCallbacks<PunManager>, IEventListener<AuthenticationEvent>
{
    private const string gameVersion = "1";
    private GUIMainMenuManager GUI;
    private bool isMatchMaking;
    public bool IsInMatchMakingRoom => isMatchMaking && PhotonNetwork.InRoom;

    //MatchMaking
    GameMode gameMode;
    GameMap gameMap;
    public Action<int> onPlayersChanged;
    
    private bool hasWelcomeed = false;

    private void Start()
    {
        GUI = GUIMainMenuManager.Instance;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Update()
    {
        GUI.SetLoadingText(PhotonNetwork.NetworkClientState.ToString());
    }

    private void Connect()
    {
        StartCoroutine(Reconnection());
    }

    IEnumerator Reconnection()
    {
        int maxAttempts = 10;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            yield return new WaitForSeconds(2f);

            if (!PhotonNetwork.IsConnected || PhotonNetwork.NetworkClientState == ClientState.ConnectingToMasterServer)
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
                attempts++;
            }
            else if (PhotonNetwork.IsConnected)
            {
                // Successfully connected, break the loop
                break;
            }
        }

        if (attempts >= maxAttempts && !PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("Failed to connect to Photon after multiple attempts");
        }
    }

    #region Pun Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        GUI.OnJoinedLobby();
        Debug.Log("Joined Lobby");
        // PhotonNetwork.LoadLevel(Scene.SceneName.PvE.ToString());
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GUI.OnRoomListUpdate(roomList);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        GUI.SetFullRoomVisibility(true);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        GUI.SetPlayerReady(targetPlayer.ActorNumber, targetPlayer.IsReadyInLobby());
        LocalPlayerPropertiesUpdated();
    }

    public override void OnJoinedRoom()
    {
        onPlayersChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        LocalPlayerPropertiesUpdated();
        JoinMatchGame();
        onPlayersChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        LocalPlayerPropertiesUpdated();
        onPlayersChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }


    public override void OnLeftRoom()
    {
        PhotonNetwork.LocalPlayer.SetReadyInLobby(false);
        GUI.OnLeftRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new Hashtable
            {
                { GlobalString.IS_IN_MATCHMAKING, true },
                { GlobalString.GAME_MODE, gameMode },
                { GlobalString.TEAM_SIZE, 2 }
            },
            CustomRoomPropertiesForLobby = new string[]
                { GlobalString.GAME_MODE, GlobalString.TEAM_SIZE, GlobalString.IS_IN_MATCHMAKING },
        };

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    #endregion

    #region Helper Methods

    public void FindMatch(GameMode gameMode, GameMap gameMap)
    {
        this.gameMode = gameMode;
        var roomProperties = new Hashtable
        {
            { GlobalString.IS_IN_MATCHMAKING, true },
            { GlobalString.GAME_MODE, gameMode },
        };
        PhotonNetwork.JoinRandomRoom(roomProperties, 0);
        isMatchMaking = true;
    }

    public void CancelFindMatch()
    {
        PhotonNetwork.LeaveRoom();
        isMatchMaking = false;
    }

    private void JoinMatchGame()
    {
        if (PhotonNetwork.IsMasterClient &&
            PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers && IsInMatchMakingRoom)
        {
            StartCoroutine(DelayedJoinMatchGame());
        }
    }

    //TODO: Need some cleaner way to do
    IEnumerator DelayedJoinMatchGame()
    {
        var players = PhotonNetwork.CurrentRoom.Players;
        while (true)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers) yield return null;
            var allPlayersReady = true;
            foreach (var player in players)
            {
                if (player.Value.GetTeam() == null)
                {
                    allPlayersReady = false;
                }
            }

            if (allPlayersReady)
            {
                JoinGame();
                yield return null;
            }

            yield return new WaitForSeconds(.5f);
        }
    }

    public void CreateCustomRoom(string roomName, int maxPlayers, int teamSize, GameMode gameMode)
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)maxPlayers,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new Hashtable
            {
                { GlobalString.GAME_MODE, gameMode },
                { GlobalString.TEAM_SIZE, teamSize },
                { GlobalString.IS_IN_MATCHMAKING, false }
            }
        };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void AddPlayerDisPlayInRoom(Player player, GameObject prefab, Transform parent)
    {
        GUI.AddPlayerElement(player, prefab, parent);
    }

    public void RemovePlayerDisplayInRoom(int actorNumber)
    {
        GUI.RemovePlayerElement(actorNumber);
    }

    public PlayerElement GetPlayerElement(int playerActorNumber)
    {
        return GUI.GetPlayerElement(playerActorNumber);
    }

    private bool CheckAllPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient) return false;
        // if (PhotonNetwork.PlayerList.Length < 2) return false;

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            if (!player.Value.IsReadyInLobby())
                return false;
        }

        return true;
    }

    private void LocalPlayerPropertiesUpdated()
    {
        if (CheckAllPlayersReady())
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (var player in PhotonNetwork.CurrentRoom.Players)
                {
                    player.Value.SetReadyInLobby(false);
                }
            }

            JoinGame();
        }
    }

    private static void JoinGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        var gameMode = PhotonNetwork.CurrentRoom.GetGameMode();
        if (gameMode == GameMode.DeathMatch)
            PhotonNetwork.LoadLevel(nameof(Scene.SceneName.Deathmatch));
        else
            PhotonNetwork.LoadLevel(nameof(Scene.SceneName.CaptureTheFlag));
    }

    #endregion

    public void OnEvent(AuthenticationEvent e)
    {
        switch (e.EventType)
        {
            case AuthenticationEventType.LoginSuccessful:
                PhotonNetwork.NickName = e.User.DisplayName;
                GUI.SetPlayerName(PhotonNetwork.NickName);
                var authValues = new AuthenticationValues
                {
                    UserId = e.User.UserId,
                };
                PhotonNetwork.AuthValues = authValues;
                Debug.Log("Check ID " + PhotonNetwork.AuthValues.UserId);
                Connect();
                break;
            case AuthenticationEventType.LogoutSuccessful:
                Debug.Log("Logout successful");
                PhotonNetwork.Disconnect();
                break;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        this.StartListening();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        this.StopListening();
    }
}