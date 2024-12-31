using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[Serializable]
public class MultiKill
{
    public string Message;
    public int BonusScore;
}

public enum TeamCount
{
    TwoTeams = 2,
    ThreeTeams = 3,
    FourTeams = 4
}

public class LevelManagerOnlineMatch : LevelManager, IEventListener<InGameEvent>
{
    [SerializeField] private PlayerCharacter playerPrefab;
    [SerializeField] private int gameDuration = 180;
    [SerializeField] private int preStartDuration = 5;
    [SerializeField] private int respawnDuration = 5;

    private PlayerCharacter localPlayer;
    private GUIManagerOnlineMatch guiManager;
    private FeedbacksManager feedbackManager;
    private EnvironmentManager environmentManager;

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
    }

    protected override void Initialize()
    {
        guiManager = GUIManagerOnlineMatch.Instance;
        feedbackManager = FeedbacksManager.Instance;
        environmentManager = EnvironmentManager.Instance;
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
            .OnStart(() => guiManager.SetVisibleDeadMask(true))
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
                SpawnPlayer();
                GameEvent.Trigger(GameEventType.GameStart);
                GameEvent.Trigger(GameEventType.GameRunning);
            })
            .OnDone(() => guiManager.SetVisibleDeadMask(false));
    }

    public override PlayerCharacter GetPlayer(string playerID)
    {
        return localPlayer;
    }

    protected override void CheckForGameOver()
    {
        if (GameManager.Instance.CurrentGameType != GameEventType.GameOver) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PhotonNetwork.LeaveRoom();
            GameEvent.Trigger(GameEventType.GameMainMenu);
        }
    }

    private void DetermineWinner()
    {
        var gameMode = PhotonNetwork.CurrentRoom.GetGameMode();

        if (gameMode is GameMode.CaptureTheFlag or GameMode.TeamDeathMatch)
            DetermineWinnerCaptureTheFlag();
        else if (gameMode == GameMode.DeathMatch)
            DetermineWinnerDeathmatch();
    }

    private void DetermineWinnerDeathmatch()
    {
        var players = PhotonNetwork.CurrentRoom.Players;
        var winnerTmp = players.First();
        bool isDraw = players.Count > 1;
        foreach (var player in players)
        {
            if (player.Value.GetScore() > winnerTmp.Value.GetScore())
            {
                winnerTmp = player;
                isDraw = false;
            }
            else if (player.Value.GetScore() < winnerTmp.Value.GetScore())
            {
                isDraw = false;
            }
        }

        if (isDraw)
        {
            guiManager.SetVisibleDrawScreen(true);
            return;
        }


        if (Equals(winnerTmp.Value, PhotonNetwork.LocalPlayer))
        {
            guiManager.SetVisibleWinScreen(true);
        }
        else
        {
            guiManager.SetVisibleLoseScreen(true);
        }
    }

    private void DetermineWinnerCaptureTheFlag()
    {
        var teams = Team.GetAllTeams().Where(team => team.Players.Count > 0).ToList();

        if (teams.Count == 0)
        {
            guiManager.SetVisibleDrawScreen(true);
            return;
        }

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

        if (winner.TeamType == TeamType.None)
        {
            isDraw = true;
        }

        if (isDraw)
        {
            guiManager.SetVisibleDrawScreen(true);
        }
        else if (winner.TeamType == PhotonNetwork.LocalPlayer.GetTeam().TeamType)
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
        var spawnPoint = GetSpawnPointForTeam();
        localPlayer = PhotonNetwork.Instantiate(GameManager.Instance.SelectedCharacter.CharacterPrefab.name, spawnPoint,
                Quaternion.identity)
            .GetComponent<PlayerCharacter>();
        CharacterEvent.Trigger(CharacterEventType.CharacterSpawned, localPlayer);
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
        guiManager.SetVisibleGameTimerText(false);
        DetermineWinner();
    }

    #endregion

    private Vector3 GetSpawnPointForTeam()
    {
        return GetValidSpawnPoint();
    }

    private Vector3 GetValidSpawnPoint()
    {
        var player = PhotonNetwork.LocalPlayer;
        var index = player.ActorNumber;
        var spawnPoint = environmentManager.CurrentMap.GetSpawnPositionByIndex(player.GetTeam(), index);

        var currentLoop = 0;

        while (!IsSpawnPointValid(spawnPoint) && currentLoop < 5)
        {
            index++;
            spawnPoint = environmentManager.CurrentMap.GetSpawnPositionByIndex(player.GetTeam(), index);
            currentLoop++;
        }

        return spawnPoint;
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
        var gameMode = PhotonNetwork.CurrentRoom.GetGameMode();
        if (gameMode == GameMode.CaptureTheFlag || gameMode == GameMode.TeamDeathMatch)
        {
            var teams = Team.GetAllTeams();
            foreach (var team in teams)
            {
                if (team.Players.Count == 0)
                    GameEvent.Trigger(GameEventType.GameOver);
            }
        }
        else if (gameMode == GameMode.DeathMatch && PhotonNetwork.CurrentRoom.Players.Count == 1)
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
            respawnTimer.ReStart();
            changedProps[GlobalString.PLAYER_DIED] = false;
        }

        if (changedProps.ContainsKey(GlobalString.PLAYER_KILLS) ||
            changedProps.ContainsKey(GlobalString.PLAYER_DEATHS) || changedProps.ContainsKey(GlobalString.PLAYER_SCORE))
        {
            UpdateScoreboard(targetPlayer);
        }
    }

    #endregion

    public void OnEvent(InGameEvent e)
    {
        switch (e.EventType)
        {
            case InGameEventType.SomeoneDied:
                UpdatePlayerStats(e.killer, e.victim);
                break;
        }
    }

    private void UpdatePlayerStats(Character killer, Character victim)
    {
        if (!Equals(localPlayer.PhotonView.Owner, killer.PhotonView.Owner)) return;
        victim.SetPlayerDied(true);
        victim.AddDeath(1);
        victim.AddScore(-1);

        killer.AddKill(1);
        killer.AddScore(killer.GetMultiKillScore());
    }

    public override void OnEnable()
    {
        base.OnEnable();
        this.StartListening<InGameEvent>();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        this.StopListening<InGameEvent>();
    }
}