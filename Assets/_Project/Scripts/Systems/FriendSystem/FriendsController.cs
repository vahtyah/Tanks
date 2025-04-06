using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class FriendsController : MonoBehaviourPunCallbacks, IEventListener<AuthenticationEvent>
{
    [SerializeField] private FriendsView view;
    [Range(1, 10)] public int UpdateRate = 2;
    [Range(2, 100)] public int MaxFriends = 50;

    [Log] private List<PhotonFriendData> friends = new();
    // private void Update()
    // {
    //     if (!PhotonNetwork.IsConnectedAndReady) return;
    //
    //     var bytesReceived = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesIn;
    //     var bytesSent = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesOut;
    //
    //     // Calculate data transfer rate (bytes/sec)
    //     float receiveRate = bytesReceived / Time.deltaTime;
    //     float sendRate = bytesSent / Time.deltaTime;
    //
    //     // Log rates in KB/s for better readability
    //     if (Debug.isDebugBuild)
    //     {
    //         Debug.Log($"Network stats - Receive: {receiveRate / 1024:F2} KB/s, Send: {sendRate / 1024:F2} KB/s");
    //     }
    //
    //     // Reset counters after processing
    //     PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsReset();
    // }

    private void Initialize()
    {
        if (!DatabaseManager.Instance.AuthenticationService.IsLoggedIn)
            return;
        view.Initialize(this);
        SubscribeFromFriendService();
        UpdateViewWithFriendsList();
        // view.SetVisible(true);
    }

    private void CleanUp()
    {
        view.CleanUp();
        friends.Clear();

        UnsubscribeFromFriendService();
    }

    private void UnsubscribeFromFriendService()
    {
        if (DatabaseManager.Instance?.FriendService != null)
        {
            DatabaseManager.Instance.FriendService.OnFriendsListChanged -= UpdateFriendsList;
            DatabaseManager.Instance.FriendService.StopListeningForFriendsChanges();
        }
    }

    private void SubscribeFromFriendService()
    {
        DatabaseManager.Instance.FriendService.OnFriendsListChanged += UpdateFriendsList;
        DatabaseManager.Instance.FriendService.StartListeningForFriendsChanges();
    }

    private void UpdateFriendsList(List<FriendData> updatedFriends)
    {
        friends.Clear();
        if (updatedFriends.Count == 0)
        {
            UpdateViewWithFriendsList();
            return;
        }

        foreach (var friend in updatedFriends)
        {
            friends.Add(new PhotonFriendData
            {
                UserId = friend.UserId,
                DisplayName = friend.DisplayName,
            });
        }

        FindFriends();
    }

    private void UpdateViewWithFriendsList()
    {
        var onlineFriends = new List<PhotonFriendData>();
        var offlineFriends = new List<PhotonFriendData>();

        foreach (var friend in friends)
        {
            if (friend.Info is { IsOnline: true })
                onlineFriends.Add(friend);
            else
                offlineFriends.Add(friend);
        }

        view.UpdateFriendsList(onlineFriends, offlineFriends);
    }

    public void AddFriend(string friendName)
    {
        if (string.IsNullOrEmpty(friendName))
            return;

        if (friends.Count >= MaxFriends)
        {
            NotificationEvent.Trigger("Friends", "Maximum friends limit reached!");
            return;
        }

        if (friends.Exists(x => x.DisplayName == friendName))
        {
            NotificationEvent.Trigger("Friends", "This user is already in your friends list.");
            return;
        }

        if (friendName == DatabaseManager.Instance.AuthenticationService.CurrentUser?.DisplayName)
        {
            NotificationEvent.Trigger("Friends", "You can't add yourself as a friend.");
            return;
        }

        view.SetLoadingMessage("Adding friend...");
        DatabaseManager.Instance.FriendService.SendFriendRequest(friendName, (success, errorMessage) =>
        {
            NotificationEvent.Trigger("Friends",
                success ? $"Friend added: {friendName}" : $"Failed to add friend: {errorMessage}");
        });
    }

    public void RemoveFriend(string friendName)
    {
        var index = friends.FindIndex(x => x.DisplayName == friendName);
        if (index == -1) return;

        view.SetLoadingMessage("Removing friend...");
        DatabaseManager.Instance.FriendService.RemoveFriend(friendName, (success, errorMessage) =>
        {
            NotificationEvent.Trigger("Friends",
                success ? $"Friend removed: {friendName}" : $"Failed to remove friend: {errorMessage}");
        });
    }

    private void FindFriends()
    {
        if (friends.Count <= 0 || !PhotonNetwork.InLobby)
            return;
        PhotonNetwork.FindFriends(friends.Select(x => x.UserId).ToArray());
    }

    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        base.OnFriendListUpdate(friendList);
        UpdateFriendsList(friendList);
    }

    private void UpdateFriendsList(List<FriendInfo> newInfo)
    {
        if (newInfo.Count == 0) return;

        foreach (var info in newInfo)
        {
            var existingFriend = friends.Find(x => x.UserId == info.UserId);
            if (existingFriend != null)
            {
                existingFriend.Info = info;
            }
        }

        UpdateViewWithFriendsList();
    }

    public void OnEvent(AuthenticationEvent e)
    {
        switch (e.EventType)
        {
            case AuthenticationEventType.LoginSuccessful:
                Initialize();
                break;
            case AuthenticationEventType.LogoutSuccessful:
                CleanUp();
                break;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        this.StartListening<AuthenticationEvent>();
        InvokeRepeating(nameof(FindFriends), 0, UpdateRate);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        this.StopListening<AuthenticationEvent>();
        CancelInvoke(nameof(FindFriends));
    }
}