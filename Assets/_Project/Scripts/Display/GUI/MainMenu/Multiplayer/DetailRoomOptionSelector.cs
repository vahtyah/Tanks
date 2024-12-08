using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DetailRoomOptionSelector : CardOptionSelector
{
    [SerializeField] private CustomInputField roomNameInputField;
    [SerializeField] private CustomInputField passwordInputField;
    [SerializeField] private SelectorCustom numberOfTeamsToggle;
    [SerializeField] private SelectorCustom numberOfMaxPlayersToggle;
    [SerializeField] private Toggle isCreateBotsToggle;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button cancelButton;
    
    
    public override void Initialize(List<CardData> cardData, int defaultIndex, UnityAction<int> onSelectionChanged)
    {
        roomNameInputField.Text = (PhotonNetwork.NickName + "'s Room");
        numberOfTeamsToggle.SetOption(numberOfTeamsToggle.GetOptions().Length - 1);
        numberOfMaxPlayersToggle.SetOption(numberOfMaxPlayersToggle.GetOptions().Length - 1);
    }
    
    public string GetRoomName()
    {
        return roomNameInputField.Text;
    }
    
    public string GetPassword()
    {
        return passwordInputField.Text;
    }
    
    public int GetNumberOfTeams()
    {
        return numberOfTeamsToggle.GetOption();
    }
    
    public int GetNumberOfMaxPlayers()
    {
        return numberOfMaxPlayersToggle.GetOption();
    }
    
    public bool IsCreateBots()
    {
        return isCreateBotsToggle.isOn;
    }
    
    public void SetCreateRoomButtonListener(UnityAction action)
    {
        createRoomButton.onClick.AddListener(action);
    }
    
    public void SetRoomName(string roomName)
    {
        roomNameInputField.Text = roomName;
    }
}
