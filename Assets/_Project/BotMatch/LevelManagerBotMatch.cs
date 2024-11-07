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

    //TODO: Instance the player Character if needed
    [SerializeField] private PlayerCharacter character;

    protected override void Initialize()
    {
        base.Initialize();
        Score = 0;
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

    public void AddOnGameEventListener(Action<int> setScoreText) { onScoreChange += setScoreText; }
}