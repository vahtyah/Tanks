using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomController : MonoBehaviour, IEventListener<RoomEvent>
{
    [SerializeField] private Button buttonOpenRoomSettings;

    [SerializeField] private RoomSettings roomSettings;
    [SerializeField] private BoardTeamPanel boardTeamPanel;

    private string roomName;
    private int teamSize;
    private int playerPerTeam;

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

    public void OnJoinRoom()
    {
        boardTeamPanel.gameObject.SetActive(true);
        var teams = Team.GetTeams();
        boardTeamPanel.Initialize(teams);
    }

    private void OnCreatedRoom(Room room)
    {
        room.SetCustomProperties(new Hashtable
        {
            { GlobalString.TEAM_SIZE, teamSize }
        });
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
                OnJoinRoom();
                break;
            case RoomEventType.PlayerJoin:
                OnPlayerJoin();
                break;
        }
    }

    private void OnPlayerJoin()
    {
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