using System;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour, IEventListener<GameEvent>, IEventListener<CharacterEvent>
{
    [SerializeField] private string playerID = "Player1";
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private CanvasGroup deadMask;
    [SerializeField] private TextMeshProUGUI spawnerCountdownText;

    [SerializeField] private CanvasGroup loseScreen;
    [SerializeField] private CanvasGroup winScreen;
    [SerializeField] private CanvasGroup drawScreen;
    
    private void Start()
    {
        playerName.text = playerID;
        SetVisibleDeadMask(false);
        SetVisibleWinScreen(false);
        SetVisibleLoseScreen(false);
        SetVisibleDrawScreen(false);
    }

    // public void OnEvent(CharacterEvent e)
    // {
    //     if (e.Character == null || !e.Character.PhotonView.IsMine) return;
    //     switch (e.EventType)
    //     {
    //         case CharacterEventType.CharacterDeath:
    //             var dead = e.Character as PlayerCharacter;
    //             if (dead == null) return;
    //             if (dead.PlayerID == playerID)
    //             {
    //                 if (deadMask == null) return;
    //                 deadMask.gameObject.SetActive(true);
    //             }
    //             break;
    //     }
    // }
    //
    // public void OnEvent(GameEvent e)
    // {
    //     switch (e.EventType)
    //     {
    //         case GameEventType.GameOver:
    //             var winner = LevelManager.Instance.GetPlayer(playerID);
    //             if (winner.PlayerID == playerID)
    //             {
    //                 if (winScreen == null) return;
    //                 winScreen.gameObject.SetActive(true);
    //             }
    //             break;
    //     }
    // }
    public void SetVisibleDeadMask(bool b) { deadMask.gameObject.SetActive(b); }

    public void SetVisibleWinScreen(bool b) { winScreen.gameObject.SetActive(b); }

    public void SetVisibleLoseScreen(bool b) { loseScreen.gameObject.SetActive(b); }

    public void SetVisibleDrawScreen(bool b) { drawScreen.gameObject.SetActive(b); }

    public void SetSpawnerCountdownText(float remaining) { spawnerCountdownText.text = remaining.ToString("0"); }

    private void OnEnable()
    {
        this.StartListening<GameEvent>();
        this.StartListening<CharacterEvent>();
    }

    private void OnDisable()
    {
        this.StopListening<GameEvent>();
        this.StopListening<CharacterEvent>();
    }

    public void OnEvent(GameEvent e) { }

    public void OnEvent(CharacterEvent e) { }
}