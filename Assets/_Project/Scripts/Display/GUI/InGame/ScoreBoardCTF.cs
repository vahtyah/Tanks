using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoardCTF : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI teamNameText;
    [SerializeField] private ScoreboardItem scoreboardItem;
    [SerializeField] private Transform container;
    Dictionary<string, ScoreboardItem> scoreboardItems = new();

    private Team team;

    public void SetTeamName(string teamName)
    {
        teamNameText.text = teamName;
    }

    public void GenerateScoreboardItem(TeamType teamType)
    {
        team = Team.GetTeamByType(teamType);
        var members = team.Players;
        foreach (var member in members)
        {
            var item = Instantiate(scoreboardItem, container);
            item.SetData(member.NickName, 0);
            scoreboardItems.Add(member.NickName, item);
        }
    }

    public void UpdateScoreboardItem(string name, int score)
    {
        SetTeamName(team.TeamType.ToString() + " " + team.GetTeamScore());
        if (scoreboardItems.TryGetValue(name, out var scoreboardItem))
        {
            scoreboardItem.SetData(name, score);
        }
    }
}