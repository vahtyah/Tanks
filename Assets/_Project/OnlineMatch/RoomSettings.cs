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

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        roomNameInputField.text = "Room Name";
        maxTeamSizeInputField.text = "2";
        maxPlayerPerTeamInputField.text = "2";
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
    
    public void SetCreateRoomButtonListener(UnityEngine.Events.UnityAction action)
    {
        createRoomButton.onClick.AddListener(action);
    }
}