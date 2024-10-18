using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ScoreboardPanel : MonoBehaviour
{
    [SerializeField] private Transform scoreboardContent;
    [SerializeField] private GameObject scoreboardItemPrefab;
    
    private readonly Dictionary<string, ScoreboardItem> scoreboardItems = new();

    public void Initialize()
    {
        var players = PhotonNetwork.PlayerList;
        foreach (var player in players)
        {
            var scoreboardItem = AddScoreboardItem(player.NickName, 0, 0, 0);
            scoreboardItems.Add(player.NickName, scoreboardItem);
        }
    }

    private ScoreboardItem AddScoreboardItem(string name, int skills, int deaths, int score)
    {
        var scoreboardItem = ScoreboardItem.Create(scoreboardItemPrefab, scoreboardContent, name, skills, deaths, score);
        if(PhotonNetwork.LocalPlayer.NickName == name)
            scoreboardItem.SetColor(Color.red);
        return scoreboardItem;
    }
    
    public void UpdateScoreboardItem(string name, int skills, int deaths, int score)
    {
        if (scoreboardItems.TryGetValue(name, out var scoreboardItem))
        {
            scoreboardItem.SetData(name, skills, deaths, score);
        }
    }
    
    public void ClearScoreboard()
    {
        foreach (Transform child in scoreboardContent)
        {
            Pool.Despawn(child.gameObject);
        }
    }
}
