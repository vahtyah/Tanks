using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[Serializable]
public class MultiKill
{
    public string message;
    public int bonusScore;
}

public class LevelManagerOnlineMatch : LevelManager
{
    [SerializeField] private PlayerCharacter playerPrefab;
    [SerializeField] private int gameDuration = 180;
    [SerializeField] private int preStartDuration = 5;
    [SerializeField] private int respawnDuration = 5;

    // [SerializeField] private float minimumSpawnDistance = 2f;
    [SerializeField] private List<Transform> spawnPoints;

    

    private ISpawnPoint spawner;
    private PlayerCharacter localPlayer;
    private GUIManagerOnlineMatch guiManager;
    private FeedbacksManager feedbackManager;
    private TeamManager teamManager;

    private Timer respawnTimer;
    private Timer gameTimer;
    private Timer preStartTimer;
    private int previousSecond;

    protected override void PreInitialize()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Scene.Load(Scene.SceneName.MainMenu);
        }


        gameDuration = GameManager.Instance.gameTime == 0 ? gameDuration : GameManager.Instance.gameTime;
        respawnDuration = GameManager.Instance.respawnTime == 0 ? respawnDuration : GameManager.Instance.respawnTime;
    }

    protected override void Initialize()
    {
        guiManager = GUIManagerOnlineMatch.Instance;
        feedbackManager = FeedbacksManager.Instance;
        teamManager = TeamManager.Instance;
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
        preStartTimer = Timer.Register(preStartDuration)
            .OnStart(() =>
            {
                guiManager.SetVisiblePreStartPanel(true);
                guiManager.SetPreStartTimerText(preStartDuration);
                guiManager.SwitchWaitingForOthersToTimer();
            })
            .OnTimeRemaining(remaining =>
            {
                int currentSecond = Mathf.CeilToInt(remaining);
                if (currentSecond != previousSecond)
                {
                    if (currentSecond == 0)
                        feedbackManager.PlayCountdownEndFeedbacks();
                    else
                        feedbackManager.PlayCountdownFeedbacks();
                    previousSecond = currentSecond;
                    guiManager.SetPreStartTimerText(currentSecond);
                }
            })
            .OnComplete(() =>
            {
                guiManager.SetVisiblePreStartPanel(false);
                GameEvent.Trigger(GameEventType.GameRunning);
            });

        gameTimer = Timer.Register(gameDuration)
            .OnStart(() => guiManager.SetVisibleGameTimerText(true))
            .OnTimeRemaining(remaining => guiManager.SetGameTimerText(remaining))
            .OnComplete(() => { GameEvent.Trigger(GameEventType.GameOver); });

        respawnTimer = Timer.Register(respawnDuration)
            .OnTimeRemaining(remaining =>
            {
                int currentSecond = Mathf.CeilToInt(remaining);
                if (currentSecond != previousSecond)
                {
                    if (currentSecond == 0)
                        feedbackManager.PlayCountdownEndFeedbacks();
                    else
                        feedbackManager.PlayCountdownSpawnFeedbacks();
                    previousSecond = currentSecond;
                    guiManager.SetSpawnerCountdownText(currentSecond);
                }
            })
            .OnComplete(() =>
            {
                guiManager.SetVisibleDeadMask(false);
                SpawnPlayer();
                GameEvent.Trigger(GameEventType.GameStart);
                GameEvent.Trigger(GameEventType.GameRunning);
            });
    }

    public override PlayerCharacter GetPlayer(string playerID)
    {
        return localPlayer;
    }

    protected override void CheckForGameOver()
    {
        if (GameManager.Instance.currentGameType != GameEventType.GameOver) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PhotonNetwork.LeaveRoom();
            GameEvent.Trigger(GameEventType.GameMainMenu);
        }
    }

    private void DetermineWinner()
    {
        // var players = PhotonNetwork.PlayerList;
        // var winnerTmp = players[0];
        // bool isDraw = players.Length > 1;
        // foreach (var player in players)
        // {
        //     if ((int)player.CustomProperties[GlobalString.PLAYER_KILLS] >
        //         (int)winnerTmp.CustomProperties[GlobalString.PLAYER_KILLS])
        //     {
        //         winnerTmp = player;
        //         isDraw = false;
        //     }
        //     else if ((int)player.CustomProperties[GlobalString.PLAYER_KILLS] <
        //              (int)winnerTmp.CustomProperties[GlobalString.PLAYER_KILLS])
        //     {
        //         isDraw = false;
        //     }
        // }
        //
        // if (isDraw)
        // {
        //     guiManager.SetVisibleDrawScreen(true);
        //     return;
        // }
        //
        // if (winnerTmp.IsLocal)
        // {
        //     guiManager.SetVisibleWinScreen(true);
        // }
        // else
        // {
        //     guiManager.SetVisibleLoseScreen(true);
        // }

        var teams = Team.GetAllTeams();
        var winner = teams[0];
        bool isDraw = teams.Count > 1;
        foreach (var team in teams)
        {
            if (team.GetTeamScore() > winner.GetTeamScore())
            {
                winner = team;
                isDraw = false;
            }
            else if (team.GetTeamScore() < winner.GetTeamScore())
            {
                isDraw = false;
            }
        }

        if (isDraw)
        {
            guiManager.SetVisibleDrawScreen(true);
            return;
        }


        if (winner.TeamType == PhotonNetwork.LocalPlayer.GetTeam().TeamType)
        {
            guiManager.SetVisibleWinScreen(true);
        }
        else
        {
            guiManager.SetVisibleLoseScreen(true);
        }
    }

    private void CheckIfAllPlayersReady()
    {
        var players = PhotonNetwork.CurrentRoom.Players.Values;
        foreach (var player in players)
        {
            if (!player.IsReadyInGame()) return;
        }

        CancelInvoke(nameof(CheckIfAllPlayersReady));
        GameEvent.Trigger(GameEventType.GameStart);
    }

    #region Game States

    protected override void GamePreStart()
    {
        PhotonNetwork.LocalPlayer.SetReadyInGame(true);
        SpawnPlayer();
        // photonView.RPC(nameof(SpawnFlag), RpcTarget.AllBuffered);
        InvokeRepeating(nameof(CheckIfAllPlayersReady), 0, .1f);
    }

    private void SpawnPlayer()
    {
        var team = PhotonNetwork.LocalPlayer.GetTeam();
        var spawnPoint = GetSpawnPointForTeam(team);
        localPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity)
            .GetComponent<PlayerCharacter>();
        // photonView.RPC(nameof(AssignTeam), RpcTarget.AllBuffered, team);
    }

    [PunRPC]
    private void AssignTeam(TeamType teamType)
    {
        localPlayer.SetTeam(teamType);
    }

    protected override void GameStart()
    {
        preStartTimer.Start();
    }

    protected override void GameRunning()
    {
        gameTimer.Start();
    }

    protected override void GameOver()
    {
        base.GameOver();
        respawnTimer.Cancel();
        guiManager.SetVisibleDeadMask(false);
        guiManager.SetVisibleGameTimerText(false);
        DetermineWinner();
    }

    #endregion

    private Vector3 GetSpawnPointForTeam(Team team)
    {
        var col = teamManager.GetSpawnArea(team.TeamType).GetComponent<Collider>();
        spawner = new AreaSpawnPoint(col);
        return GetValidSpawnPoint();
    }

    private Vector3 GetValidSpawnPoint()
    {
        int i = 0;
        while (i < 10)
        {
            i++;
            var spawnPoint = spawner.NextSpawnPointByIndex(PhotonNetwork.LocalPlayer.ActorNumber);
            if (IsSpawnPointValid(spawnPoint))
                return spawnPoint;
        }

        return spawner.NextRandomSpawnPoint();
    }

    private bool IsSpawnPointValid(Vector3 spawnPoint)
    {
        var bounds = playerPrefab.GetComponent<Collider>().bounds;
        Collider[] results = new Collider[10];
        var size = Physics.OverlapBoxNonAlloc(spawnPoint, bounds.extents, results, Quaternion.identity,
            LayerMask.GetMask("Player"));
        for (int i = 0; i < size; i++)
        {
            if (results[i].gameObject == localPlayer.gameObject)
                continue;
            return false;
        }

        return true;
    }

    private GameObject GetPlayerCharacter(Player player)
    {
        return player.TagObject as GameObject;
    }

    protected override void HandleCharacterDeath(Character character)
    {
    }

    private void UpdateScoreboard(Player targetPlayer)
    {
        var props = targetPlayer.CustomProperties;
        guiManager.UpdateScoreboard(targetPlayer.NickName, (int)props[GlobalString.PLAYER_KILLS],
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

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        CheckPlayerLeftRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(Scene.SceneName.MainMenu.ToString());
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(GlobalString.PLAYER_DIED) && (bool)changedProps[GlobalString.PLAYER_DIED] &&
            Equals(localPlayer.PhotonView.Owner, targetPlayer))
        {
            respawnTimer.Reset();
            guiManager.SetVisibleDeadMask(true);
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