using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerName : MonoBehaviour, IEventListener<GameEvent>
{
    private TextMeshProUGUI playerName;
    
    private void Awake()
    {
        playerName = GetComponent<TextMeshProUGUI>();
    }
    
    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GameStart:
                playerName.text = PhotonNetwork.LocalPlayer.NickName;
                break;
        }
    }
    
    private void OnEnable()
    {
        this.StartListening();
    }
    
    private void OnDisable()
    {
        this.StopListening();
    }
}
