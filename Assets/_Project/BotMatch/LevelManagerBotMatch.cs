using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagerBotMatch : LevelManager, IPhotonViewCallback
{
    private string username;
    private int score;

    public string Username
    {
        get => username;
        set
        {
            username = value;
            onUsernameChange?.Invoke(username);
        }
    }

    public int Score
    {
        get => score;
        private set
        {
            score = value;
            onScoreChange?.Invoke(score);
        }
    }

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

    //Event
    private Action<int> onScoreChange;
    private Action<string> onUsernameChange;

    //TODO: Instance the player Character if needed
    [SerializeField] private PlayerCharacter character;

    protected override void Awake()
    {
        base.Awake();
        // StartCoroutine(Disconnect());
        Team.Create(TeamType.Red);
        PhotonNetwork.LocalPlayer.AssignTeam(TeamType.Red);
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 1 }, null);
    }

    public override void OnJoinedRoom()
    {
        GameEvent.Trigger(GameEventType.GamePreStart);
    }

    IEnumerator Disconnect()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }

        PhotonNetwork.OfflineMode = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        guiManager = GUIManagerOnlineMatch.Instance;
        feedbackManager = FeedbacksManager.Instance;
        environmentManager = EnvironmentManager.Instance;
        RegisterTimers();
        Score = 0;
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

    private void SpawnPlayer()
    {
        var playerSpawnPoint = environmentManager.CurrentMap.GetRandomSpawnPosition();
        localPlayer = PhotonNetwork.Instantiate(GameManager.Instance.SelectedCharacter.CharacterPrefab.name, playerSpawnPoint, Quaternion.identity)
            .GetComponent<PlayerCharacter>();
        CharacterEvent.Trigger(CharacterEventType.CharacterSpawned, localPlayer);
    }

    public override PlayerCharacter GetPlayer(string playerID)
    {
        return character;
    }

    protected override void HandleCharacterDeath(Character character)
    {
        if (character is PlayerCharacter playerCharacter)
        {
            winner = playerCharacter;
            StartCoroutine(TriggerGameOverAfterDelay());
        }
        else
        {
            Score++;
        }
    }

    public void AddOnGameEventListener(Action<int> setScoreText)
    {
        onScoreChange += setScoreText;
    }

    protected override void GamePreStart()
    {
        SpawnPlayer();
        GameEvent.Trigger(GameEventType.GameStart);
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
        // DetermineWinner();
    }
}