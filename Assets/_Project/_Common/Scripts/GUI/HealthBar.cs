using System;
using UnityEngine;

public class HealthBar : MonoBehaviour, IEventListener<GameEvent>
{
    [SerializeField] private Transform foregroundBar;
    [SerializeField] private Transform delayedBarDecreasing;
    [SerializeField] private string playerID = "Player1";
    private Health health;

    private void UpdateBar(float value)
    {
        foregroundBar.localScale = new Vector3(value, 1f);
        delayedBarDecreasing.localScale = new Vector3(value, 1f);
    }

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GameStart:
                if (health == null)
                    health = LevelManager.Instance.GetPlayer(playerID).Health;
                health.AddOnHitListener(UpdateBar);
                break;
        }
    }
    
    private void OnEnable()
    {
        this.StartListening<GameEvent>();
    }
    
    private void OnDisable()
    {
        this.StopListening<GameEvent>();
    }
}