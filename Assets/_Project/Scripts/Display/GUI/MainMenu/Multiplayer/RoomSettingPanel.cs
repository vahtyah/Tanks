using System;
using UnityEngine;
using UnityEngine.UI;

public class RoomSettingPanel : MonoBehaviour
{
    private RoomSetting[] settings;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private UIMultiplayerController controller;

    private void Awake()
    {
        createRoomButton.onClick.AddListener(ButtonCreateRoomOnClick);
        cancelButton.onClick.AddListener(ButtonCancelOnClick);
    }

    private void Start()
    {
        settings = GetComponents<RoomSetting>();
        foreach (var setting in settings)
        {
            setting.Initialize();
        }
    }

    private void ButtonCreateRoomOnClick()
    {
        foreach (var setting in settings)
        {
            setting.SaveSetting();
        }
        
        controller.OnCreateRoomButtonClick();
    }
    
    private void ButtonCancelOnClick()
    {
        gameObject.SetActive(false);
    }
}
