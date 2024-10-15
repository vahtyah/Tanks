using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagerBotMatch : LevelManager
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

    //Event
    private Action<int> onScoreChange;
    private Action<string> onUsernameChange;

    //TODO: Instance the player character if needed
    [SerializeField] private PlayerCharacter character;

    protected override void Initialization()
    {
        base.Initialization();
        Score = 0;
    }

    public override PlayerCharacter GetPlayer(string playerID)
    {
        return character;
    }

    protected override void CharacterDeath(Character character)
    {
        if (character is PlayerCharacter playerCharacter)
        {
            winner = playerCharacter;
            StartCoroutine(IETriggerGameOver());
        }
        else
        {
            Score++;
        }
    }

    public void AddOnGameEventListener(Action<int> setScoreText) { onScoreChange += setScoreText; }
}