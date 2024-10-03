using TMPro;
using UnityEngine;

public class GUIManagerBotMatch : GUIManager
{
    [SerializeField] private GameObject usernamePanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreTextMaskDie;
    [SerializeField] private TextMeshProUGUI usernameText;

    protected override void Start()
    {
        base.Start();
        SetUsernamePanel(true);
    }

    public override void SetUsernamePanel(bool b)
    {
        if (usernamePanel == null) return;
        usernamePanel.SetActive(b);
    }
    
    public override void SetScoreText(int score)
    {
        if (scoreText == null) return;
        scoreText.text = score.ToString();
    }
    
    public override void SetScoreTextMaskDie(int score)
    {
        if (scoreTextMaskDie == null) return;
        scoreTextMaskDie.text = "You got " + CalculateRankingInTheScoreboard(score) + "th place with " + score + " points!";
    }
    
    private int CalculateRankingInTheScoreboard(int score)
    {
        var scoreboards = DatabaseManager.Instance.users;
        for (int i = 0; i < scoreboards.Count; i++)
        {
            Debug.Log(scoreboards[i].score);
            if (scoreboards[i].score < score)
            {
                return i + 1;
            }
        }
        return scoreboards.Count;
    }

    public override void SetUserName(string inputFieldText)
    {
        if (usernameText == null) return;
        usernameText.text = inputFieldText;
    }
}