using System;
using TMPro;
using UnityEngine;

public class GUIManagerOnlineMatch : Singleton<GUIManagerOnlineMatch>
{
    [SerializeField] private TextMeshProUGUI gameTimerText;
    [SerializeField] private GameObject preStartPanel;
    [SerializeField] private TextMeshProUGUI preStartTimerText;
    [SerializeField] private TextMeshProUGUI waitingForOthersText;
    
    //Scoreboard
    [SerializeField] private ScoreboardPanel scoreboardPanel;

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
        if(Input.GetKeyDown(KeyCode.Tab))
            SetVisibleScoreboard(true);
        else if(Input.GetKeyUp(KeyCode.Tab))
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
        scoreboardPanel.gameObject.SetActive(b);
    }
    
    public void UpdateScoreboard(string name, int skills, int deaths, int scores)
    {
        scoreboardPanel.UpdateScoreboardItem(name, skills, deaths, scores);
    }
}