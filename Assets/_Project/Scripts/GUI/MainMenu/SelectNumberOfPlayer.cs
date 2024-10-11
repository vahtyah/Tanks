﻿using UnityEngine;
using UnityEngine.UI;

public class SelectNumberOfPlayer : MonoBehaviour
{
    [SerializeField] private Button twoPlayersButton;
    [SerializeField] private Button threePlayersButton;
    [SerializeField] private Button fourPlayersButton;
    
    private void Start()
    {
        twoPlayersButton.onClick.AddListener(() => ButtonSelectAmountPlayerOnClick(2));
        threePlayersButton.onClick.AddListener(() => ButtonSelectAmountPlayerOnClick(3));
        fourPlayersButton.onClick.AddListener(() => ButtonSelectAmountPlayerOnClick(4));
    }

    private void ButtonSelectAmountPlayerOnClick(int value)
    {
        LevelManagerLocalMatch.Instance.NumberOfPlayers = value;
        GameEvent.Trigger(GameEventType.GameStart);
    }
}