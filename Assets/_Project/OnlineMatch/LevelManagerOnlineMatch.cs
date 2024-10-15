using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManagerOnlineMatch : LevelManager
{
    [SerializeField] private PlayerCharacter playerPrefab;
    [SerializeField] private int gameTimeLength = 20;
    [SerializeField] private int preStartTimeLength = 5;

    private PlayerCharacter player;
    private GUIManagerOnlineMatch GUI;

    private Timer deathTimer;
    private Timer gameTimer;
    private Timer preStartTimer;

    protected override void PreInitialization()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Scene.Load(Scene.SceneName.MainMenu);
        }
    }

    protected override void Initialization()
    {
        GUI = GUIManagerOnlineMatch.Instance;
        RegisterCustomProperties();
        GameEvent.Trigger(GameEventType.GamePreStart);
    }

    private void RegisterCustomProperties()
    {
        var props = new Hashtable
        {
            { GlobalString.PLAYER_DIED, false },
            { GlobalString.PLAYER_KILLS, 0 },
            { GlobalString.PLAYER_DEATHS, 0 },
            { GlobalString.PLAYER_SCORE, 0 }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override PlayerCharacter GetPlayer(string playerID) { return player; }

    protected override void GamePreStart()
    {
        PhotonNetwork.LocalPlayer.SetScore(0); //For determining if player is ready to play, 0 = ready, -1 = not ready
        SpawnPlayer(); //Spawn player
        RegisterTimers(); //Register timers
        InvokeRepeating(nameof(CheckIfAllPlayersReady), 1, .1f);
    }

    private void RegisterTimers()
    {
        preStartTimer = Timer.Register(preStartTimeLength)
            .OnStart(() =>
            {
                GUI.SetVisiblePreStartPanel(true);
                GUI.SetPreStartTimerText(preStartTimeLength);
                GUI.SwitchWaitingForOthersToTimer();
            })
            .OnRemaining(remaining => GUI.SetPreStartTimerText(remaining))
            .OnComplete(() =>
            {
                GUI.SetVisiblePreStartPanel(false);
                GameEvent.Trigger(GameEventType.GameStart);
            });

        gameTimer = Timer.Register(gameTimeLength)
            .OnStart(() => GUI.SetVisibleGameTimerText(true))
            .OnRemaining(remaining => GUI.SetGameTimerText(remaining))
            .OnComplete(() =>
            {
                deathTimer.Cancel();
                GUI.SetVisibleGameTimerText(false);
                GameEvent.Trigger(GameEventType.GameOver);
            });

        deathTimer = Timer.Register(5).OnComplete(() =>
        {
            SpawnPlayer();
            GameEvent.Trigger(GameEventType.GameStart);
        });
    }

    private void CheckIfAllPlayersReady()
    {
        var players = PhotonNetwork.PlayerList;
        foreach (var player in players)
        {
            if (player.GetScore() == -1) return;
        }

        CancelInvoke(nameof(CheckIfAllPlayersReady));
        preStartTimer.Start();
    }

    protected override void GameStart() { gameTimer.Start(); }

    private void SpawnPlayer()
    {
        player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity)
            .GetComponent<PlayerCharacter>();
    }

    protected override void CharacterDeath(Character character) { }

    private void UpdateScoreboard(Player targetPlayer)
    {
        var props = targetPlayer.CustomProperties;
        GUI.UpdateScoreboard(targetPlayer.NickName, (int)props[GlobalString.PLAYER_KILLS],
            (int)props[GlobalString.PLAYER_DEATHS], (int)props[GlobalString.PLAYER_SCORE]);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(GlobalString.PLAYER_DIED) && (bool)changedProps[GlobalString.PLAYER_DIED] &&
            Equals(player.PhotonView.Owner, targetPlayer))
        {
            deathTimer.Reset();
            changedProps[GlobalString.PLAYER_DIED] = false;
        }

        if (changedProps.ContainsKey(GlobalString.PLAYER_KILLS) ||
            changedProps.ContainsKey(GlobalString.PLAYER_DEATHS) || changedProps.ContainsKey(GlobalString.PLAYER_SCORE))
        {
            UpdateScoreboard(targetPlayer);
        }
    }
}