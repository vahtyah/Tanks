using System;
using TMPro;
using UnityEngine;

public class GUIManagerBotMatch : Singleton<GUIManagerBotMatch>, IEventListener<GameEvent>
{
    [SerializeField] private GameObject usernamePanel;
    [SerializeField] protected GameObject pausePanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreTextMaskDie;
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private GameObject diePanel;
    private LevelManagerBotMatch levelManagerBotMatch;

    private void Start()
    {
        levelManagerBotMatch = (LevelManagerBotMatch)LevelManager.Instance;
        if(levelManagerBotMatch== null)
        {
            throw new Exception("LevelManagerBotMatch is null");
        }
        SetUsernamePanel(true);
        levelManagerBotMatch.AddOnGameEventListener(SetScoreText);
    }

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GamePreStart:
                SetUsernamePanel(true);
                SetPausePanel(false);
                break;
            case GameEventType.GameStart:
                SetUserName(levelManagerBotMatch.Username);
                SetPausePanel(false);
                SetUsernamePanel(false);
                break;
            case GameEventType.GamePause:
                SetPausePanel(true);
                break;
            case GameEventType.GameOver:
                SetScoreTextMaskDie(levelManagerBotMatch.Score);
                SetDiePanel(true);
                break;
        }        
    }
    
    private void SetPausePanel(bool b)
    {
        if (pausePanel == null) return;
        pausePanel.SetActive(b);
    }

    private void SetUsernamePanel(bool b)
    {
        if (usernamePanel == null) return;
        usernamePanel.SetActive(b);
    }

    private void SetDiePanel(bool b)
    {
        if (diePanel == null) return;
        diePanel.SetActive(b);
    }

    private void SetScoreText(int score)
    {
        if (scoreText == null) return;
        scoreText.text = score.ToString();
    }

    private void SetScoreTextMaskDie(int score)
    {
        if (scoreTextMaskDie == null) return;
        scoreTextMaskDie.text = "You got " + CalculateRankingInTheScoreboard(score) + "th place with " + score + " points!";
    }
    
    private int CalculateRankingInTheScoreboard(int score)
    {
        var scoreboards = DatabaseManager.Instance.users;
        for (int i = 0; i < scoreboards.Count; i++)
        {
            if (scoreboards[i].score < score)
            {
                return i + 1;
            }
        }
        return scoreboards.Count;
    }

    private void SetUserName(string inputFieldText)
    {
        if (usernameText == null) return;
        usernameText.text = inputFieldText;
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