using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntry : MonoBehaviour
{
    public static PlayerEntry Create(GameObject prefab, string playerName, int ownerID, Transform parent)
    {
        var entry = Instantiate(prefab, parent).GetComponent<PlayerEntry>();
        entry.SetPlayerName(playerName);
        entry.SetReadyButton(false);
        entry.SetOwner(ownerID);
        return entry;
    }
    
    public static void Remove(PlayerEntry entry)
    {
        Destroy(entry.gameObject);
    }

    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] Button readyButton;
    
    [SerializeField] private Image buttonImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image blurImage;
    
    public bool isReady;
    private int ownerId;

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

    private void SetPlayerName(string name) { playerName.text = name; }

    private void SetOwner(int id) { ownerId = id; }

    public void SetReadyButton(bool isReady)
    {
        if (isReady)
        {
            buttonImage.color = Color.green;
            borderImage.color = Color.green;
            
            Color color = Color.green;
            color.a = 0.3f;
            
            blurImage.color = color;
            readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready!";
        }
        else
        {
            buttonImage.color = Color.red;
            borderImage.color = Color.red;
            Color color = Color.red;
            color.a = 0.3f;
            
            blurImage.color = color;
            readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready?";
        }
    }
}