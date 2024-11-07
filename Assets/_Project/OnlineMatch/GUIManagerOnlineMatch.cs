using System;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;

public class GUIManagerOnlineMatch : SerializedSingleton<GUIManagerOnlineMatch>, IEventListener<GameEvent>
{
    [SerializeField] private TextMeshProUGUI gameTimerText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject preStartPanel;
    [SerializeField] private TextMeshProUGUI preStartTimerText;
    [SerializeField] private TextMeshProUGUI waitingForOthersText;

    //Scoreboard
    [OdinSerialize] private IScoreboardPanel scoreboardPanel;

    [SerializeField] private PlayerHUD playerHUD;
    [SerializeField] private FillAmountImage targetIndicator;

    private void Start()
    {
        SetVisibleGameTimerText(false);
        SetVisiblePreStartPanel(true);
        SwitchWaitingForOthersToTimer(false);
        scoreboardPanel.Initialize();
        SetVisibleScoreboard(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            SetVisibleScoreboard(true);
        else if (Input.GetKeyUp(KeyCode.Tab))
            SetVisibleScoreboard(false);
    }

    public void SetGameTimerText(float time)
    {
        string minutes = Mathf.Floor(time / 60).ToString("00");
        string seconds = (time % 60).ToString("00");
        gameTimerText.text = $"{minutes}:{seconds}";
    }

    public void SetVisibleGameTimerText(bool b)
    {
        gameTimerText.gameObject.SetActive(b);
    }

    public void SetVisiblePreStartPanel(bool b)
    {
        preStartPanel.gameObject.SetActive(b);
    }

    public void SetPreStartTimerText(float remaining)
    {
        preStartTimerText.text = remaining.ToString("0");
    }

    public void SwitchWaitingForOthersToTimer(bool b = true)
    {
        waitingForOthersText.gameObject.SetActive(!b);
        preStartTimerText.gameObject.SetActive(b);
    }

    private void SetVisibleScoreboard(bool b)
    {
        scoreboardPanel.SetVisible(b);
    }

    public void UpdateScoreboard(string name, int skills, int deaths, int scores)
    {
        scoreboardPanel.UpdateScoreboardItem(name, skills, deaths, scores);
    }

    private void SetVisiblePausePanel(bool b)
    {
        if (pausePanel == null) return;
        pausePanel.SetActive(b);
    }

    public void SetVisibleDeadMask(bool b)
    {
        playerHUD.SetVisibleDeadMask(b);
    }

    public void SetVisibleWinScreen(bool b)
    {
        playerHUD.SetVisibleWinScreen(b);
    }

    public void SetVisibleLoseScreen(bool b)
    {
        playerHUD.SetVisibleLoseScreen(b);
    }

    public void SetVisibleDrawScreen(bool b)
    {
        playerHUD.SetVisibleDrawScreen(b);
    }

    public void SetSpawnerCountdownText(float remaining)
    {
        playerHUD.SetSpawnerCountdownText(remaining);
    }

    public void SetTargetIndicatorActive(bool b)
    {
        targetIndicator.gameObject.SetActive(b);
    }

    public void SetTargetIndicatorPosition(Vector3 position)
    {
        targetIndicator.transform.position = position;
    }

    public void SetCaptureProgress(float obj)
    {
        targetIndicator.SetFillAmount(obj);
    }

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GamePause:
                SetVisiblePausePanel(true);
                break;
            case GameEventType.GameRunning:
                SetVisiblePausePanel(false);
                break;
        }
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