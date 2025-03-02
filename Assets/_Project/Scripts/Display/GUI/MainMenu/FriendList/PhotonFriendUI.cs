using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonFriendUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image statusImage;
    [SerializeField] private GameObject joinButton;
    
    private PhotonFriendList manager;
    private FriendData cacheData;
    
    int lastStatus = -1;
    
    public bool Set(FriendData data, PhotonFriendList from)
    {
        cacheData = data;
        gameObject.name = data.Name;
        manager = from;
        nameText.text = data.Name;
        string st = manager.StatusData[0].Status;
        Color sc = manager.StatusData[0].Color;
        int status = 0;
        joinButton.SetActive(false);
        if (data.Info != null)
        {
            if (data.Info.IsInRoom)
            {
                status = 1;
                st = manager.StatusData[status].Status;
                sc = manager.StatusData[status].Color;
                joinButton.SetActive(true);
            }
            else if (data.Info.IsOnline)
            {
                status = 2;
                st = manager.StatusData[status].Status;
                sc = manager.StatusData[status].Color;
            }
        }
        statusText.text = st;
        statusImage.color = sc;

        if(lastStatus == -1) { lastStatus = status; }
        else
        {
            if (status == lastStatus) return false;
            lastStatus = status; return true;
        }

        return false;
    }
}

