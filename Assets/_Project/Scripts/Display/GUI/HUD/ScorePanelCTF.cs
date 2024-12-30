using System.Collections.Generic;
using UnityEngine;

public class ScorePanelCTF : MonoBehaviour, IScoreboardPanel
{
    [SerializeField] private GameObject ScoreBoardPrefab;
    [SerializeField] private Transform container;
    
    [SerializeField] private Dictionary<TeamType, ScoreBoardCTF> scoreBoards = new();
    
    public void Initialize()
    {
        var teams = Team.GetAllTeams();
        foreach (var team in teams)
        {
            var scoreBoard = Instantiate(ScoreBoardPrefab, container).GetComponent<ScoreBoardCTF>();
            scoreBoard.SetTeamName(team.TeamType.ToString());
            scoreBoard.GenerateScoreboardItem(team.TeamType);
            scoreBoards.Add(team.TeamType, scoreBoard);
        }
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void UpdateScoreboardItem(string name, int skills, int deaths, int score)
    {
        foreach (var scoreBoard in scoreBoards)
        {
            scoreBoard.Value.UpdateScoreboardItem(name, score);
        }
    }
}
