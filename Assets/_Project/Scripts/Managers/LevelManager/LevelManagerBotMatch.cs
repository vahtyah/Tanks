using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagerBotMatch : Singleton<LevelManagerBotMatch>, IEventListener<CharacterEvent>
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

    private PlayerCharacter winner;
    protected bool isGameOver;

    //Event
    private Action<int> onScoreChange;
    private Action<string> onUsernameChange;

    //TODO: Instance the player character if needed
    [SerializeField] private PlayerCharacter character;

    private void Start()
    {
        Score = 0;
    }
    private void Update() { CheckForGameOver(); }

    private void CheckForGameOver()
    {
        if (!isGameOver) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameEvent.Trigger(GameEventType.GamePreStart);
            Scene.Load(SceneManager.GetActiveScene().name);
        }
    }

    public PlayerCharacter GetPlayer() { return character; }

    private void CharacterDeath(Character character)
    {
        if (character is PlayerCharacter playerCharacter)
        {
            winner = playerCharacter;
            isGameOver = true;
            StartCoroutine(IETriggerGameOver());
        }
        else
        {
            Score++;
        }
    }

    public void OnEvent(CharacterEvent e)
    {
        switch (e.EventType)
        {
            case CharacterEventType.CharacterDeath:
                CharacterDeath(e.Character);
                break;
        }
    }

    private IEnumerator IETriggerGameOver()
    {
        yield return new WaitForSecondsRealtime(1);
        isGameOver = true;
        GameEvent.Trigger(GameEventType.GameOver);
    }

    private void OnEnable() { this.StartListening(); }

    private void OnDisable() { this.StopListening(); }

    public void AddOnGameEventListener(Action<int> setScoreText) { onScoreChange += setScoreText; }
}