using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuPanel : MonoBehaviour
{
    [Header("Buttons")] [SerializeField] private Button resume;
    [SerializeField] private Button exit;

    [Header("Panels")] [SerializeField] private GameObject pauseMenuPanel;

    private void Awake()
    {
        resume.onClick.AddListener(OnResume);
        exit.onClick.AddListener(OnExit);
    }

    private void OnExit()
    {
        Photon.Pun.PhotonNetwork.LeaveRoom();
    }

    private void OnResume()
    {
        GameEvent.Trigger(GameEventType.TogglePause);
    }
}