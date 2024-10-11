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

    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] Button readyButton;
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
                var props = new Hashtable() { { GlobalString.PLAYER_READY, isReady } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            });
        }
    }

    private void SetPlayerName(string name) { playerName.text = name; }

    private void SetOwner(int id) { ownerId = id; }

    public void SetReadyButton(bool isReady)
    {
        if (isReady)
        {
            readyButton.GetComponent<Image>().color = Color.green;
            readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready!";
        }
        else
        {
            readyButton.GetComponent<Image>().color = Color.red;
            readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready?";
        }
    }
}