using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI playerCount;
    [SerializeField] private TextMeshProUGUI hostName;
    [SerializeField] private TextMeshProUGUI ping;
    [SerializeField] private Button joinButton;
    public void Initialize(RoomInfo info)
    {
        roomName.text = info.Name;
        
        var idHost = info.masterClientId;
        // var host = PhotonNetwork.CurrentRoom.GetPlayer(idHost);
        hostName.text = info.masterClientId.ToString();
        playerCount.text = $"{info.PlayerCount}/{info.MaxPlayers}";
        ping.text = "0";
        joinButton.onClick.AddListener(() =>
        {
            if(!PhotonNetwork.IsConnectedAndReady) return;
            PhotonNetwork.JoinRoom(info.Name);
        });
    }

    public void UpdateInfo(RoomInfo room)
    {
        playerCount.text = $"{room.PlayerCount}/{room.MaxPlayers}";
        
        var idHost = room.masterClientId;
        // var host = PhotonNetwork.CurrentRoom.GetPlayer(idHost);
        hostName.text = room.masterClientId.ToString();
    }
}