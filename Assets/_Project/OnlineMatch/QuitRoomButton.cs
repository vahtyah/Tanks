using UnityEngine;
using UnityEngine.UI;

public class QuitRoomButton : MonoBehaviour
{
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(QuitRoom);
    }
    
    private void QuitRoom()
    {
        Photon.Pun.PhotonNetwork.LeaveRoom();
    }
}
