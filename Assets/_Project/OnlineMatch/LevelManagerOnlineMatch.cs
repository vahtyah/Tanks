using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

public class LevelManagerOnlineMatch : LevelManager
{
    [SerializeField] private PlayerCharacter playerPrefab;
    [SerializeField] private int gameTimeLength = 20;
    [SerializeField] private int preStartTimeLength = 5;
    [SerializeField] private int deathTimeLength = 5;
    [SerializeField] private float minimumDistanceToSpawn = 2f;
    [SerializeField] private List<Transform> spawnPoints;
    
    private Dictionary<string, List<PlayerCharacter>> teamPlayers = new();

    private ISpawnPoint spawner;
    private PlayerCharacter player;
    private GUIManagerOnlineMatch GUI;
    private FeedbacksManager Feedbacks;

    private Timer deathTimer;
    private Timer gameTimer;
    private Timer preStartTimer;
    private int previousSecond;

    protected override void PreInitialization()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Scene.Load(Scene.SceneName.MainMenu);
        }
        
        
        gameTimeLength = GameManager.Instance.gameTime == 0 ? gameTimeLength : GameManager.Instance.gameTime;
        deathTimeLength = GameManager.Instance.respawnTime == 0 ? deathTimeLength : GameManager.Instance.respawnTime;
    }

    protected override void Initialization()
    {
        GUI = GUIManagerOnlineMatch.Instance;
        Feedbacks = FeedbacksManager.Instance;
        spawner = new LinearSpawnPoint(spawnPoints);
        RegisterCustomProperties();
        RegisterTimers();
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

    private void RegisterTimers()
    {
        preStartTimer = Timer.Register(preStartTimeLength)
            .OnStart(() =>
            {
                GUI.SetVisiblePreStartPanel(true);
                GUI.SetPreStartTimerText(preStartTimeLength);
                GUI.SwitchWaitingForOthersToTimer();
            })
            .OnRemaining(remaining =>
            {
                int currentSecond = Mathf.CeilToInt(remaining);
                if (currentSecond != previousSecond)
                {
                    if (currentSecond == 0)
                        Feedbacks.PlayCountdownEndFeedbacks();
                    else
                        Feedbacks.PlayCountdownFeedbacks();
                    previousSecond = currentSecond;
                    GUI.SetPreStartTimerText(currentSecond);
                }
            })
            .OnComplete(() =>
            {
                GUI.SetVisiblePreStartPanel(false);
                GameEvent.Trigger(GameEventType.GameRunning);
            });

        gameTimer = Timer.Register(gameTimeLength)
            .OnStart(() => GUI.SetVisibleGameTimerText(true))
            .OnRemaining(remaining => GUI.SetGameTimerText(remaining))
            .OnComplete(() => { GameEvent.Trigger(GameEventType.GameOver); });

        deathTimer = Timer.Register(deathTimeLength)
            .OnRemaining(remaining =>
            {
                int currentSecond = Mathf.CeilToInt(remaining);
                if (currentSecond != previousSecond)
                {
                    if (currentSecond == 0)
                        Feedbacks.PlayCountdownEndFeedbacks();
                    else
                        Feedbacks.PlayCountdownSpawnFeedbacks();
                    previousSecond = currentSecond;
                    GUI.SetSpawnerCountdownText(currentSecond);
                }
            })
            .OnComplete(() =>
            {
                GUI.SetVisibleDeadMask(false);
                SpawnPlayer();
                GameEvent.Trigger(GameEventType.GameStart);
                GameEvent.Trigger(GameEventType.GameRunning);
            });
    }

    public override PlayerCharacter GetPlayer(string playerID) { return player; }

    protected override void CheckForGameOver()
    {
        if (GameManager.Instance.currentGameType != GameEventType.GameOver) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PhotonNetwork.LeaveRoom();
            GameEvent.Trigger(GameEventType.GameMainMenu);
        }
    }

    private void SetWinner()
    {
        var players = PhotonNetwork.PlayerList;
        var winnerTmp = players[0];
        bool isDraw = players.Length > 1;
        foreach (var player in players)
        {
            if ((int)player.CustomProperties[GlobalString.PLAYER_KILLS] >
                (int)winnerTmp.CustomProperties[GlobalString.PLAYER_KILLS])
            {
                winnerTmp = player;
                isDraw = false;
            }
            else if ((int)player.CustomProperties[GlobalString.PLAYER_KILLS] <
                     (int)winnerTmp.CustomProperties[GlobalString.PLAYER_KILLS])
            {
                isDraw = false;
            }
        }

        if (isDraw)
        {
            GUI.SetVisibleDrawScreen(true);
            return;
        }

        if (winnerTmp.IsLocal)
        {
            GUI.SetVisibleWinScreen(true);
        }
        else
        {
            GUI.SetVisibleLoseScreen(true);
        }
    }

    private void CheckIfAllPlayersReady()
    {
        var players = PhotonNetwork.PlayerList;
        foreach (var player in players)
        {
            if (player.GetScore() == -1) return;
        }

        CancelInvoke(nameof(CheckIfAllPlayersReady));
        GameEvent.Trigger(GameEventType.GameStart);
    }

    #region Game States

    protected override void GamePreStart()
    {
        PhotonNetwork.LocalPlayer.SetScore(0); //For determining if player is ready to play, 0 = ready, -1 = not ready
        SpawnPlayer(); //Spawn player
        InvokeRepeating(nameof(CheckIfAllPlayersReady), 0, .1f);
    }

    protected override void GameStart() { preStartTimer.Start(); }

    protected override void GameRunning() { gameTimer.Start(); }

    protected override void GameOver()
    {
        base.GameOver();
        deathTimer.Cancel();
        GUI.SetVisibleDeadMask(false);
        GUI.SetVisibleGameTimerText(false);
        SetWinner();
    }

    #endregion

    private void SpawnPlayer()
    {
        var spawnPoint = spawner.NextSpawnPointByIndex(PhotonNetwork.LocalPlayer.ActorNumber);
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity)
            .GetComponent<PlayerCharacter>();
        // player = Pool.PhotonSpawn(playerPrefab.gameObject, spawnPoint, Quaternion.identity).GetComponent<PlayerCharacter>();
    }

    private Vector3 GetValidPoint()
    {
        int i = 0;
        while (i < 10)
        {
            i++;
            var spawnPoint = spawner.NextSpawnPointByIndex(player.PhotonView.Owner.ActorNumber);
            if (IsSpawnPointValid(spawnPoint))
                return spawnPoint;
        }

        return spawner.NextRandomSpawnPoint();
    }

    private bool IsSpawnPointValid(Vector3 spawnPoint)
    {
        var players = PhotonNetwork.CurrentRoom.Players.Values.Select(GetPlayerCharacter).ToList();
        foreach (var player in players)
        {
            if (player == null) continue;
            if (Vector3.Distance(player.transform.position, spawnPoint) < minimumDistanceToSpawn)
                return false;
        }

        return true;
    }

    private GameObject GetPlayerCharacter(Player player) { return player.TagObject as GameObject; }
    protected override void CharacterDeath(Character character) { }

    private void UpdateScoreboard(Player targetPlayer)
    {
        var props = targetPlayer.CustomProperties;
        GUI.UpdateScoreboard(targetPlayer.NickName, (int)props[GlobalString.PLAYER_KILLS],
            (int)props[GlobalString.PLAYER_DEATHS], (int)props[GlobalString.PLAYER_SCORE]);
    }

    private void CheckPlayerLeftRoom()
    {
        if (PhotonNetwork.PlayerList.Length == 1 && GameManager.Instance.currentGameType != GameEventType.GameOver)
        {
            GameEvent.Trigger(GameEventType.GameOver);
        }
    }

    #region Photon calls

    public override void OnPlayerLeftRoom(Player otherPlayer) { CheckPlayerLeftRoom(); }

    public override void OnLeftRoom() { PhotonNetwork.LoadLevel(Scene.SceneName.MainMenu.ToString()); }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(GlobalString.PLAYER_DIED) && (bool)changedProps[GlobalString.PLAYER_DIED] &&
            Equals(player.PhotonView.Owner, targetPlayer))
        {
            deathTimer.Reset();
            GUI.SetVisibleDeadMask(true);
            changedProps[GlobalString.PLAYER_DIED] = false;
        }

        if (changedProps.ContainsKey(GlobalString.PLAYER_KILLS) ||
            changedProps.ContainsKey(GlobalString.PLAYER_DEATHS) || changedProps.ContainsKey(GlobalString.PLAYER_SCORE))
        {
            UpdateScoreboard(targetPlayer);
        }
    }

    #endregion
}