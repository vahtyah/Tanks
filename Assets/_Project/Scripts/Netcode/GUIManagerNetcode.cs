using System;
using TMPro;
using UnityEngine;

public class GUIManagerNetcode : Singleton<GUIManagerNetcode>, IEventListener<GameEvent>
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pickRolePanel;
    [SerializeField] private GameObject diePanel;
    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private GameObject hostDisconnectedPanel;
    [SerializeField] private HealthBarNetwork healthBar;

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GamePreStart:
                SetDiePanel(false);
                SetPausePanel(false);
                SetPickRolePanel(true);
                break;
            case GameEventType.GameStart:
                SetPausePanel(false);
                SetPickRolePanel(false);
                break;
            case GameEventType.GameOver:
                SetDiePanel(true);
                break;
            case GameEventType.GamePause:
                SetPausePanel(true);
                break;
        }
    }
    
    public void SetPausePanel(bool b)
    {
        if (pausePanel == null)
        {
            Debug.LogWarning("PausePanel is null");
            return;
        }

        pausePanel.SetActive(b);
    }

    private void SetPickRolePanel(bool b)
    {
        if (pickRolePanel == null)
        {
            Debug.LogWarning("PickRolePanel is null");
            return;
        }

        pickRolePanel.SetActive(b);
    }

    public void SetDiePanel(bool b)
    {
        if (diePanel == null)
        {
            Debug.LogWarning("DiePanel is null");
            return;
        }

        diePanel.SetActive(b);
    }
    
    public void SetHealthBar(HealthNetwork health)
    {
        if (healthBar == null)
        {
            Debug.LogWarning("HealthBar is null");
            return;
        }

        healthBar.SetHealth(health);
    }
    
    public void SetWinnerPanel(bool b)
    {
        if (winnerPanel == null)
        {
            Debug.LogWarning("WinnerPanel is null");
            return;
        }

        winnerPanel.SetActive(b);
    }
    
    public void SetHostDisconnectedPanel(bool b)
    {
        if (hostDisconnectedPanel == null)
        {
            Debug.LogWarning("HostDisconnectedPanel is null");
            return;
        }

        hostDisconnectedPanel.SetActive(b);
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
