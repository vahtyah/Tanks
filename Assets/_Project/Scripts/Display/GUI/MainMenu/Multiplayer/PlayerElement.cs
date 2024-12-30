using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerElement : MonoBehaviour
{
    public static PlayerElement Create(GameObject prefab, Player player, Transform parent)
    {
        var entry = Instantiate(prefab, parent).GetComponent<PlayerElement>();
        entry.SetPlayerName(player.NickName);
        entry.SetReadyButton(false);
        entry.SetOwner(player.ActorNumber);
        return entry;
    }

    public static void Remove(PlayerElement element)
    {
        Destroy(element.gameObject);
    }

    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] Button readyButton;
    [SerializeField] private Image background;
    
    [SerializeField] private Image buttonImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image blurImage;

    [SerializeField] private Color readyColor;
    private Color notReadyColor;
    

    public bool isReady;
    private int ownerId;

    private void Awake()
    {
        notReadyColor = background.color;

    }

    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        {
            readyButton.interactable = false;
        }
        else
        {
            readyButton.onClick.AddListener(() =>
            {
                isReady = !isReady;
                SetReadyButton(isReady);
                PhotonNetwork.LocalPlayer.SetReadyInLobby(isReady);
            });
        }
    }

    private void SetPlayerName(string name)
    {
        playerName.text = name;
    }

    private void SetOwner(int id)
    {
        ownerId = id;
    }

    public void SetReadyButton(bool isReady)
    {
        if (isReady)
        {
            // buttonImage.color = Color.green;
            // borderImage.color = Color.green;

            background.color = readyColor;

            // blurImage.color = color;
            // readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready!";
        }
        else
        {
            // buttonImage.color = Color.red;
            // borderImage.color = Color.red;
            // Color color = Color.red;
            // color.a = 0.3f;
            //
            // blurImage.color = color;
            // readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready?";
            
            background.color = notReadyColor;
        }
    }
}