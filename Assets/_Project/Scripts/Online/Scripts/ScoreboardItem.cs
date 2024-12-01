using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardItem : MonoBehaviour
{
    public static ScoreboardItem Create(GameObject prefab, Transform parent, string name, int skills, int deaths, int score)
    {
        var scoreboardItem = Instantiate(prefab, parent).GetComponent<ScoreboardItem>();
        scoreboardItem.SetData(name, skills, deaths, score);
        return scoreboardItem;
    }
    
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI skills;
    [SerializeField] private TextMeshProUGUI deaths;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Image background;
    
    public void SetData(string name, int skills, int deaths, int score)
    {
        playerName.text = name;
        this.skills.text = skills.ToString();
        this.deaths.text = deaths.ToString();
        this.score.text = score.ToString();
    }

    public void SetData(string name, int score)
    {
        playerName.text = name;
        this.score.text = score.ToString();
    }
    
    public void SetColor(Color color)
    {
        background.color = color;
    }
}
