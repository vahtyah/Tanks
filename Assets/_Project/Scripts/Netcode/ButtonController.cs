using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private TMP_InputField inputField;


    private void Awake()
    {
        hostButton.onClick.AddListener(Host);
        clientButton.onClick.AddListener(Client);
    }

    private void OnEnable()
    {
        notificationText.gameObject.SetActive(false);
    }

    private void Server()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            notificationText.gameObject.SetActive(true);
            notificationText.text = "Server is already running";
            return;
        }

        NetworkManager.Singleton.StartServer();
        notificationText.gameObject.SetActive(true);
        notificationText.text = "Server started";
    }

    private void Host()
    {
        // Kiểm tra nếu host đã khởi động (bằng cách kiểm tra IsServer và IsHost)
        // if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        // {
        //     notificationText.gameObject.SetActive(true);
        //     notificationText.text = "Server is already running";
        //     return;
        // }

        // Bắt đầu host
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(IPManager.Instance.GetLocalIPAddress(), 7777);
        NetworkManager.Singleton.StartHost();

        notificationText.gameObject.SetActive(true);
        notificationText.text = "Host started";
        GameEvent.Trigger(GameEventType.GameStart);
    }

    private void Client()
    {
        // Kiểm tra nếu server chưa khởi động
        // if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost)
        // {
        //     notificationText.gameObject.SetActive(true);
        //     notificationText.text = "Server is not running";
        //     return;
        // }

        Debug.Log("Client is starting...");
        // if (inputField.text != IPManager.Instance.GetLocalIPAddress())
        // {
        //     notificationText.gameObject.SetActive(true);
        //     notificationText.text = "Invalid IP address";
        //     return;
        // }
        
        
        
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(inputField.text, 7777);
        NetworkManager.Singleton.StartClient();
        notificationText.gameObject.SetActive(true);
        notificationText.text = "Client started";
        GameEvent.Trigger(GameEventType.GameStart);
    }
}
