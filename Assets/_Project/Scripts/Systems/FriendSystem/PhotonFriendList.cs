using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

[Serializable]
public class FriendData
{
    public string Name;
    public FriendInfo Info;
    public PhotonFriendUI UI;
}

[Serializable]
public class FriendUIStatus
{
    public string Status;
    public Color Color;
}
public class PhotonFriendList : MonoBehaviourPunCallbacks
{
    [Header("Settings")]
    [Range(1, 10)] public int UpdateRate = 2;
    [Range(2, 100)] public int MaxFriends = 50;
    public FriendUIStatus[] StatusData;
    
    [Header("References")]
    public GameObject Content;
    public GameObject FriendUIPrefab;
    public RectTransform Separator;
    public RectTransform NoFriendsOnline;
    public RectTransform ListPanel;
    public TextMeshProUGUI FriendsCountText;
    
    private List<FriendData> friendsArray = new();
    private bool pendingRebuild = false;


    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
        Content.SetActive(false);
    }
    
    public void LoadFriends()
    {
        string data = PlayerPrefs.GetString(FRIENDS_KEY, string.Empty);
        friendsArray.Clear();
        if (!string.IsNullOrEmpty(data))
        {
            string[] all = data.Split("|"[0]);
            for (int i = 0; i < all.Length; i++)
            {
                if (string.IsNullOrEmpty(all[i])) continue;

                FriendData fd = new();
                fd.Name = all[i];
                friendsArray.Add(fd);
            }
            if (PhotonNetwork.IsConnectedAndReady)
            {
                //update UI list
                PhotonNetwork.FindFriends(friendsArray.Select(x => x.Name).ToArray());
            }
        }
    }

    public void UpdateUIList(bool rebuild = false)
    {
        if (friendsArray.Count <= 0) return;

        if (rebuild || pendingRebuild)
        {
            ClearUI();
            bool isSomeOneOnline = false;
            List<FriendData> offLineList = new List<FriendData>();
            //instance online friends first
            for (int i = 0; i < friendsArray.Count; i++)
            {
                if(friendsArray[i].Info == null || !friendsArray[i].Info.IsOnline)
                {
                    offLineList.Add(friendsArray[i]);
                    continue;
                }
                GameObject g = Instantiate(FriendUIPrefab, ListPanel, false) as GameObject;
                friendsArray[i].UI = g.GetComponent<PhotonFriendUI>();
                friendsArray[i].UI.Set(friendsArray[i], this);
                isSomeOneOnline = true;
            }
            NoFriendsOnline.gameObject.SetActive(!isSomeOneOnline);
            Separator.SetAsLastSibling();
            for (int i = 0; i < offLineList.Count; i++)
            {
                GameObject g = Instantiate(FriendUIPrefab, ListPanel, false) as GameObject;
                offLineList[i].UI = g.GetComponent<PhotonFriendUI>();
                offLineList[i].UI.Set(offLineList[i], this);
            }
            FriendsCountText.text = $"{friendsArray.Count - offLineList.Count}/{friendsArray.Count}";
            pendingRebuild = false;
        }
        else
        {
            for (int i = 0; i < friendsArray.Count; i++)
            {
                if (friendsArray[i].UI == null) continue;

                //if the status has change
                if(friendsArray[i].UI.Set(friendsArray[i], this))
                {
                    pendingRebuild = true;
                }
            }
        }
    }
    
    void UpdateFriendArray(List<FriendInfo> newList)
    {
        bool hasChanges = false;
        for (int i = 0; i < newList.Count; i++)
        {
            if (friendsArray.Exists(x => x.Name == newList[i].UserId))
            {
                int index = friendsArray.FindIndex(x => x.Name == newList[i].UserId);
                friendsArray[index].Info = newList[i];
                if(friendsArray[index].UI == null)
                {
                    hasChanges = true;
                }
            }
            else
            {
                FriendData fd = new FriendData();
                fd.Name = newList[i].UserId;
                fd.Info = newList[i];
                friendsArray.Add(fd);
                hasChanges = true;
            }
        }
        //update UI list
        UpdateUIList(hasChanges);
    }
    
    public void AddFriend(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        if((friendsArray.Count + 1) >= MaxFriends)
        {
            return;
        }
        if(friendsArray.Exists(x => x.Name == text))
        {
            return;
        }
        if(text == PhotonNetwork.NickName)
        {
            return;
        }
        //apply any other filter that you want here

        var newFriend = new FriendData
        {
            Name = text
        };
        friendsArray.Add(newFriend);
        SaveFriends();
        UpdateUIList(true);
    }
    
    public void RemoveFriend(string Name)
    {
        int index = friendsArray.FindIndex(x => x.Name == Name);
        if (index == -1) return;

        if(friendsArray[index].UI != null) { Destroy(friendsArray[index].UI.gameObject); }
        friendsArray.RemoveAt(index);
        SaveFriends();
    }
    
    public void SaveFriends()
    {
        string[] array = friendsArray.Select(x => x.Name).ToArray();
        string line = string.Join("|", array);
        PlayerPrefs.SetString(FRIENDS_KEY, line);
    }
    
    void FindFriends()
    {
        if (friendsArray.Count <= 0) return;
        if (!PhotonNetwork.InLobby) return;

        PhotonNetwork.FindFriends(friendsArray.Select(x => x.Name).ToArray());
    }
    
    public void AddFriend(TMP_InputField field)
    {
        AddFriend(field.text);
        field.text = string.Empty;
        field.DeactivateInputField();
    }
    
    void ClearUI()
    {
        foreach (FriendData e in friendsArray)
        {
            if(e.UI != null)
            {
                Destroy(e.UI.gameObject);
            }
        }
    }

    #region Photon Callbacks

    public override void OnJoinedLobby()
    {
        //load save friends
        LoadFriends();
        InvokeRepeating("FindFriends", 1, UpdateRate);
        Content.SetActive(true);
    }
    
    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        UpdateFriendArray(friendList);
    }
    
    public override void OnJoinedRoom()
    {
        CancelInvoke("FindFriends");
        ClearUI();
        Content.SetActive(false);
    }

    #endregion

    private string FRIENDS_KEY => $"{PhotonNetwork.NickName}.{Application.identifier}.{"friends"}";
}