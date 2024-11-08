using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PunManager : SingletonPunCallbacks<PunManager>
{
    private const string gameVersion = "1";
    private GUIMainMenuManager GUI;

    private void Start()
    {
        GUI = GUIMainMenuManager.Instance;
        
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = "Player_" + Random.Range(0, 1000);
        GUI.SetPlayerName(PhotonNetwork.NickName);
        GUI.RegisterPlayerNameInputListener((value) => { PhotonNetwork.NickName = value; });
        
        OnLoginButtonClicked();
    }

    private void Update()
    {
        GUI.SetLoadingText(PhotonNetwork.NetworkClientState.ToString());
    }

    private void OnLoginButtonClicked()
    {
        StartCoroutine(Reconnection());
    }

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

    #region Pun Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        GUI.OnJoinedLobby();
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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        LocalPlayerPropertiesUpdated();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        LocalPlayerPropertiesUpdated();
    }


    public override void OnLeftRoom()
    {
        PhotonNetwork.LocalPlayer.SetReadyInLobby(false);
        GUI.OnLeftRoom();
    }

    #endregion

    #region Helper Methods
    
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

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.IsReadyInLobby())
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
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    player.SetReadyInLobby(false);
                }
            }

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            var gameMode = PhotonNetwork.CurrentRoom.GetGameMode();
            if (gameMode == GameMode.Deathmatch)
                PhotonNetwork.LoadLevel(Scene.SceneName.Deathmatch.ToString());
            else
                PhotonNetwork.LoadLevel(Scene.SceneName.CaptureTheFlag.ToString());
        }
    }

    #endregion
}