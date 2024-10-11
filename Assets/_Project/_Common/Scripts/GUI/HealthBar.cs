using System;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform foregroundBar;
    [SerializeField] private Transform delayedBarDecreasing;
    [SerializeField] private string playerID = "Player1";

    private Health health;

    private void Start()
    {
        health = LevelManagerBotMatch.Instance ? LevelManagerBotMatch.Instance.GetPlayer().Health : LevelManagerLocalMatch.Instance.GetPlayer(playerID).Health;

        health.AddOnHitListener(UpdateBar);
    }

    private void UpdateBar(float value)
    {
        foregroundBar.localScale = new Vector3(value, 1f);
        delayedBarDecreasing.localScale = new Vector3(value, 1f);
    }
}