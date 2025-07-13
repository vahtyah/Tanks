using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
//TODO: clear this script, mix of UIMainMenuManager and RoomManager, waitting for the final version of UI
public class UIMultiplayerController : MonoBehaviour, IEventListener<RoomEvent>
{
    [FoldoutGroup("Settings")] [SerializeField]
    private GameObject roomPrefab;

    [FoldoutGroup("Settings")] [SerializeField]
    private Transform roomContainer;

    [SerializeField] private Button buttonOpenRoomSettings;
    [SerializeField] private Button buttonCloseRoom;
    

    [SerializeField] private RoomSettingPanel roomSettingPanel;

    [SerializeField] private GameObject inRoomPanel;
    
    [SerializeField] private BoardTeamPanel boardTeamPanel;
    [SerializeField] private TeamBoard teamBoardDeathmatch;

    [SerializeField] private StandardButton readyButton;
    

    private string roomName;
    private int teamSize;
    private int playerSize;
    private GameMode gameMode;
    private bool isCreateBots;

    private Dictionary<string, RoomElement> roomList = new();
    private Dictionary<int, PlayerElement> playerList = new();
    
    public void SetInRoomPanelVisible(bool isVisible)
    {
        inRoomPanel.SetActive(isVisible);
        if(gameMode == GameMode.DeathMatch)
            teamBoardDeathmatch.gameObject.SetActive(isVisible);
        else boardTeamPanel.gameObject.SetActive(isVisible);
    }

    public void SetPlayerReady(int actorNumber, bool isReady)
    {
        if (playerList.TryGetValue(actorNumber, out var playerElement))
        {
            playerElement.SetReadyButton(isReady);
        }
    }

    public void RemovePlayerElement(int actorNumber)
    {
        if (playerList.TryGetValue(actorNumber, out var playerElement))
        {
            PlayerElement.Remove(playerElement);
            playerList.Remove(actorNumber);
        }
    }

    public void AddPlayerElement(Player player, GameObject prefab, Transform parent)
    {
        var playerElement = PlayerElement.Create(prefab, player, parent);
        playerList.Add(player.ActorNumber, playerElement);
    }
    
    public PlayerElement GetPlayerElement(int actorNumber)
    {
        return playerList.GetValueOrDefault(actorNumber);
    }
    
    public void RemoveAllPlayerElements()
    {
        foreach (var element in playerList)
        {
            PlayerElement.Remove(element.Value);
        }

        playerList.Clear();
        
        if (gameMode == GameMode.DeathMatch)
        {
            teamBoardDeathmatch.ClearMembers();
        }
        else if (gameMode == GameMode.CaptureTheFlag)
        {
            boardTeamPanel.Clear();
        }
    }

    public void RoomListUpdate(List<RoomInfo> rooms)
    {
        foreach (var room in rooms)
        {
            if(room.CustomProperties.ContainsKey(GlobalString.IS_IN_MATCHMAKING) && (bool)room.CustomProperties[GlobalString.IS_IN_MATCHMAKING])
                continue;
            if (room.RemovedFromList)
            {
                if (roomList.TryGetValue(room.Name, out var roomElement))
                {
                    RoomElement.Destroy(roomElement);
                    roomList.Remove(room.Name);
                }
            }
            else
            {
                if (roomList.TryGetValue(room.Name, out var roomElement))
                {
                    roomElement.UpdateInfo(room);
                }
                else
                {
                    var newRoomElement = RoomElement.Create(roomPrefab, roomContainer, room);
                    roomList.Add(room.Name, newRoomElement);
                }
            }
        }
    }

    private void Start()
    {
        buttonOpenRoomSettings.onClick.AddListener(() =>
        {
            SetRoomSettingsPanelVisible(true);
        });
        
        buttonCloseRoom.onClick.AddListener(() =>
        {
            PhotonNetwork.LeaveRoom();
        });
        
        readyButton.OnClick(() =>
        {
            Debug.Log(PhotonNetwork.LocalPlayer.IsReadyInLobby());
            PhotonNetwork.LocalPlayer.SetReadyInLobby(!PhotonNetwork.LocalPlayer.IsReadyInLobby());
            readyButton.ChangeText(PhotonNetwork.LocalPlayer.IsReadyInLobby() ? "Ready" : "Cancel");
        });
    }
    
    public void SetRoomName(string roomName)
    {
        this.roomName = roomName;
    }

    public void SetGameMode(GameMode gameMode)
    {
        this.gameMode = gameMode;
    }
    
    public void SetTeamSize(int teamSize)
    {
        this.teamSize = teamSize;
    }
    
    public void SetCreateBots(bool isCreateBots)
    {
        this.isCreateBots = isCreateBots;
    }
    
    public void SetPlayerSize(int playerSize)
    {
        this.playerSize = playerSize;
    }

    public void OnCreateRoomButtonClick()
    {
        roomSettingPanel.gameObject.SetActive(false);

        int maxPlayers = 0;
        
        if(gameMode == GameMode.DeathMatch)
        {
            maxPlayers = playerSize;
        }
        else if(gameMode == GameMode.CaptureTheFlag)
        {
            maxPlayers = teamSize * 4;
        }
        
        PunManager.Instance.CreateCustomRoom(roomName, maxPlayers, teamSize, gameMode);
    }

    private void SetRoomSettingsPanelVisible(bool isVisible)
    {
        roomSettingPanel.gameObject.SetActive(isVisible);
    }

    public void OnJoinRoom(Room room)
    {
        Debug.Log("Join room: " + room.Name);
        Debug.Log("GameMode: " + gameMode);
        gameMode = room.GetGameMode() == GameMode.None ? gameMode : room.GetGameMode();
        SetInRoomPanelVisible(true);
        Debug.Log("GameMode: " + gameMode);
        if (gameMode == GameMode.CaptureTheFlag)
        {
            var teams = Team.GetAllTeams();
            boardTeamPanel.Initialize(teams);
        }
        else if (gameMode == GameMode.DeathMatch)
        {
            foreach (var player in room.Players)
            {
                teamBoardDeathmatch.AddMember(player.Value);
            }
        }
    }

    private void OnCreatedRoom(Room room)
    {
        room.SetTeamSize(teamSize);
        room.SetGameMode(gameMode);
        var teams = room.CreateTeams(teamSize);
    }

    public void OnEvent(RoomEvent e)
    {
        switch (e.EventType)
        {
            case RoomEventType.RoomCreate:
                OnCreatedRoom(e.Room);
                break;
            case RoomEventType.RoomJoin:
                OnJoinRoom(e.Room);
                break;
            case RoomEventType.PlayerJoin:
                OnPlayerJoin(e.Player);
                break;
            case RoomEventType.PlayerLeft:
                OnPlayerLeft(e.Player);
                break;
            case RoomEventType.RoomLeave:
                SetInRoomPanelVisible(false);
                RemoveAllPlayerElements();
                break;
        }
    }

    private void OnPlayerLeft(Player ePlayer)
    {
        if (gameMode == GameMode.DeathMatch)
        {
            teamBoardDeathmatch.RemoveMember(ePlayer);
        }
    }

    private void OnPlayerJoin(Player newPlayer)
    {
        if (gameMode == GameMode.DeathMatch)
        {
            teamBoardDeathmatch.AddMember(newPlayer);
        }
    }

    public void OnEnable()
    {
        this.StartListening();
    }

    public void OnDisable()
    {
        this.StopListening();
    }
}