using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class GUIMainMenuManager : Singleton<GUIMainMenuManager>, IEventListener<NavigationEvent>
{
    [TabGroup("Loading")] [SerializeField] private GameObject loadingPanel;
    [TabGroup("Loading")] [SerializeField] private TextMeshProUGUI loadingText;

    [TabGroup("Main Panel")] [SerializeField]
    private MainPanelManager mainPanel;

    [TabGroup("Main Panel"), SerializeField]
    private UIMultiplayerController multiplayerUI;

    [TabGroup("Main Panel"), SerializeField]
    private UIHomeController homeUI;
    
    [TabGroup("Main Panel"), SerializeField]
    private GraphicSettingsPanel graphicSettingsUI;
    [TabGroup("Main Panel"), SerializeField]
    private AudioSettingsPanel audioSettingsUI;

    [TabGroup("Player")] [SerializeField] private TMP_InputField playerNameInput;
    [TabGroup("Window")] [SerializeField] private GameObject gameFullWindow;

    protected override void Awake()
    {
        base.Awake();
        SetLoadingPanelVisibility(true);
        SetMainPanelVisibility(false);
    }

    #region Pun Callbacks

    public void OnJoinedLobby()
    {
        SetLoadingPanelVisibility(false);
        SetMainPanelVisibility(true);
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        SetLoadingPanelVisibility(true);
        SetMainPanelVisibility(false);
    }

    public void OnLeftRoom()
    {
        multiplayerUI.RemoveAllPlayerElements();
    }

    public void OnRoomListUpdate(List<RoomInfo> rooms)
    {
        multiplayerUI.RoomListUpdate(rooms);
    }

    #endregion

    #region Loading

    public void SetLoadingPanelVisibility(bool isVisible)
    {
        loadingPanel.SetActive(isVisible);
    }

    public void SetLoadingText(string text)
    {
        if (loadingPanel.activeSelf)
            loadingText.text = text;
    }

    #endregion

    #region Main Panel

    public void SetMainPanelVisibility(bool isVisible)
    {
        mainPanel.gameObject.SetActive(isVisible);
    }

    public void HideSelectCharacterPanel()
    {
        homeUI.CharacterSelector.TurnOffDisplay();
        homeUI.CharacterSelector.SetCanChangeSelection(false);
    }

    public void ShowSelectCharacterPanel()
    {
        homeUI.CharacterSelector.TurnOnDisplay();
        homeUI.CharacterSelector.SetCanChangeSelection(true);
    }

    public void SetCanChangeSelection(bool canChange)
    {
        homeUI.CharacterSelector.SetCanChangeSelection(canChange);
    }

    #endregion

    #region Player

    public void SetPlayerName(string playerName)
    {
        playerNameInput.text = playerName;
    }

    public string GetPlayerName()
    {
        return playerNameInput.text;
    }

    public void RegisterPlayerNameInputListener(UnityEngine.Events.UnityAction<string> action)
    {
        playerNameInput.onEndEdit.AddListener(action);
    }

    public void SetPlayerReady(int actorNumber, bool isReady)
    {
        multiplayerUI.SetPlayerReady(actorNumber, isReady);
    }

    public void AddPlayerElement(Player player, GameObject prefab, Transform parent)
    {
        multiplayerUI.AddPlayerElement(player, prefab, parent);
    }

    public void RemovePlayerElement(int actorNumber)
    {
        multiplayerUI.RemovePlayerElement(actorNumber);
    }

    public PlayerElement GetPlayerElement(int actorNumber)
    {
        return multiplayerUI.GetPlayerElement(actorNumber);
    }

    #endregion

    #region Window

    public void SetFullRoomVisibility(bool isVisible)
    {
        gameFullWindow.SetActive(isVisible);
    }

    #endregion

    
    private void OnEnable()
    {
        this.StartListening();
    }
    
    private void OnDisable()
    {
        this.StopListening();
    }

    public void OnEvent(NavigationEvent e)
    {
        switch (e.NavigationType)
        {
            case NavigationType.Home:
                ShowSelectCharacterPanel();
                break;
            case NavigationType.Settings:
                SettingsManager.Instance.LoadSettings();
                graphicSettingsUI.ResetSettings();
                audioSettingsUI.ResetSettings();
                HideSelectCharacterPanel();
                break;
            default:
                HideSelectCharacterPanel();
                break;
        }
    }
}