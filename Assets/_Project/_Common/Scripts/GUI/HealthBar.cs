using System;
using UnityEngine;

public class HealthBar : MonoBehaviour, IEventListener<CharacterEvent>
{
    [SerializeField] private Transform foregroundBar;
    [SerializeField] private Transform delayedBarDecreasing;
    [SerializeField] private string playerID = "Player1";
    private HealthTest health;

    private void UpdateBar(float value)
    {
        foregroundBar.localScale = new Vector3(value, 1f);
        delayedBarDecreasing.localScale = new Vector3(value, 1f);
    }

    public void OnEvent(CharacterEvent e)
    {
        switch (e.EventType)
        {
            case CharacterEventType.CharacterSpawned:
                if (health == null)
                    health = e.Character.Health;
                health.AddHealthChangeListener(UpdateBar);
                break;
        }
    }
    
    private void OnEnable()
    {
        this.StartListening();
    }
    
    private void OnDisable()
    {
        this.StopListening();
    }
}