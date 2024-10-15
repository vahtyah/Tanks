﻿using System;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour, IEventListener<GameEvent>, IEventListener<CharacterEvent>
{
    [SerializeField] private string playerID = "Player1";
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private CanvasGroup deadMask;
    [SerializeField] private CanvasGroup winScreen;

    public void OnEvent(CharacterEvent e)
    {
        // if (e.Character == null || !e.Character.PhotonView.IsMine) return;
        // switch (e.EventType)
        // {
        //     case CharacterEventType.CharacterDeath:
        //         var dead = e.Character as PlayerCharacter;
        //         if (dead == null) return;
        //         if (dead.PlayerID == playerID)
        //         {
        //             if (deadMask == null) return;
        //             deadMask.gameObject.SetActive(true);
        //         }
        //         dead.PhotonView.Owner.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        //         {
        //             {GlobalString.PLAYER_DIED, true}
        //         });
        //         break;
        // }
    }

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GameOver:
                var winner = LevelManager.Instance.GetPlayer(playerID);
                if (winner.PlayerID == playerID)
                {
                    if (winScreen == null) return;
                    winScreen.gameObject.SetActive(true);
                }
                break;
        }
    }

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
}