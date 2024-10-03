using System;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour, IEventListener<Event>
{
    [SerializeField] private string playerID = "Player1";
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private CanvasGroup deadMask;
    [SerializeField] private CanvasGroup winScreen;

    public void OnEvent(Event e)
    {
        if(e.OriginCharacter == null) return;
        switch (e.EventType)
        {
            case EventType.PlayerDeath:
                if (e.OriginCharacter.PlayerID == playerID)
                {
                    if(deadMask == null) return;
                    deadMask.gameObject.SetActive(true);
                }
                break;
            case EventType.GameOver:
                if(e.OriginCharacter.PlayerID == playerID)
                {
                    winScreen.gameObject.SetActive(true);
                }
                break;
        }
    }

    private void OnEnable() { this.StartListening(); }

    private void OnDisable() { this.StopListening(); }
}