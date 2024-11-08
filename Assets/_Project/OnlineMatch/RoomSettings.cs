using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomSettings : MonoBehaviour
{
    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TMP_InputField maxTeamSizeInputField;
    [SerializeField] private TMP_InputField maxPlayerPerTeamInputField;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private TMP_Dropdown gameModeDropdown;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        roomNameInputField.text = "Room Name";
        maxTeamSizeInputField.text = "2";
        maxPlayerPerTeamInputField.text = "2";
        gameModeDropdown.ClearOptions();
        var options = System.Enum.GetNames(typeof(GameMode)).ToList();
        gameModeDropdown.AddOptions(options);
    }
    
    public int GetMaxTeamSize()
    {
        return int.Parse(maxTeamSizeInputField.text);
    }
    
    public int GetMaxPlayerPerTeam()
    {
        return int.Parse(maxPlayerPerTeamInputField.text);
    }
    
    public string GetRoomName()
    {
        return roomNameInputField.text;
    }
    
    public GameMode GetGameMode()
    {
        return (GameMode)gameModeDropdown.value;
    }
    
    public void SetCreateRoomButtonListener(UnityEngine.Events.UnityAction action)
    {
        createRoomButton.onClick.AddListener(action);
    }
}