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


    [SerializeField] private RoomSettings roomSettings;
    [SerializeField] private BoardTeamPanel boardTeamPanel;
    [SerializeField] private TeamBoard teamBoardDeathmatch;

    private string roomName;
    private int teamSize;
    private int playerPerTeam;

    private Dictionary<string, RoomElement> roomList = new();
    private Dictionary<int, PlayerElement> playerList = new();

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
    }

    public void RoomListUpdate(List<RoomInfo> rooms)
    {
        foreach (var room in rooms)
        {
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
        Initialize();
    }

    private void Initialize()
    {
        buttonOpenRoomSettings.onClick.AddListener(OnOpenRoomSettingsButtonClick);
        roomSettings.SetCreateRoomButtonListener(OnCreateRoomButtonClick);
    }

    private void OnCreateRoomButtonClick()
    {
        roomSettings.gameObject.SetActive(false);

        roomName = roomSettings.GetRoomName();
        teamSize = roomSettings.GetMaxTeamSize();
        playerPerTeam = roomSettings.GetMaxPlayerPerTeam();

        var roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)(teamSize * playerPerTeam),
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    private void OnOpenRoomSettingsButtonClick()
    {
        roomSettings.gameObject.SetActive(true);
    }

    public void OnJoinRoom(Room room)
    {
        var gameMode = roomSettings.GetGameMode();

        if (gameMode == GameMode.CaptureTheFlag)
        {
            boardTeamPanel.gameObject.SetActive(true);
            var teams = Team.GetAllTeams();
            boardTeamPanel.Initialize(teams);
        }
        else if (gameMode == GameMode.Deathmatch)
        {
            teamBoardDeathmatch.gameObject.SetActive(true);
            foreach (var player in room.Players)
            {
                teamBoardDeathmatch.AddMember(player.Value);
            }
        }
    }

    private void OnCreatedRoom(Room room)
    {
        room.SetTeamSize(teamSize);
        room.SetGameMode(roomSettings.GetGameMode());
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
        }
    }

    private void OnPlayerLeft(Player ePlayer)
    {
        var gameMode = roomSettings.GetGameMode();
        if (gameMode == GameMode.Deathmatch)
        {
            teamBoardDeathmatch.RemoveMember(ePlayer);
        }
    }

    private void OnPlayerJoin(Player newPlayer)
    {
        Debug.Log("Player joined");
        var gameMode = roomSettings.GetGameMode();
        if (gameMode == GameMode.Deathmatch)
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