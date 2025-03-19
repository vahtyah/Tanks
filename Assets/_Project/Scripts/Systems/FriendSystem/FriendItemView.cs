using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image statusImage;
    [SerializeField] private Button removeButton;
    [SerializeField] private Button joinButton;
    
    private FriendsController controller;
    private PhotonFriendData photonFriendData;

    private void OnEnable()
    {
        removeButton.onClick.AddListener(OnRemoveButtonClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
    }
    
    private void OnDisable()
    {
        removeButton.onClick.RemoveListener(OnRemoveButtonClicked);
        joinButton.onClick.RemoveListener(OnJoinButtonClicked);
    }

    public void Initialize(PhotonFriendData photonFriendData, FriendsController controller)
    {
        this.photonFriendData = photonFriendData;
        this.controller = controller;
        UpdateUI();
    }

    private void OnJoinButtonClicked()
    {
        if (photonFriendData.Info != null && photonFriendData.Info.IsInRoom)
        {
            // controller.JoinRoom(friendData.Info.RoomName);
        }
        else
        {
            Debug.Log("Friend is not in a room");
        }
    }

    private void OnRemoveButtonClicked()
    {
        Debug.Log("Remove friend button clicked");
        controller.RemoveFriend(photonFriendData.DisplayName);
    }

    private void UpdateUI()
    {
        nameText.text = photonFriendData.DisplayName;
        
        if(photonFriendData.Info != null)
        {
            statusText.text = photonFriendData.Info.IsOnline ? "ONLINE" : "OFFLINE";
            statusImage.color = photonFriendData.Info.IsOnline ? Color.green : Color.gray;

            if (photonFriendData.Info.IsInRoom)
            {
                joinButton.gameObject.SetActive(true);
                statusText.text = "IN ROOM";
                statusImage.color = Color.yellow;
            }
            else
            {
                joinButton.gameObject.SetActive(false);
            }
        }
        else
        {
            statusText.text = "UNKNOWN";
            statusImage.color = Color.red;
        }
    }
}

