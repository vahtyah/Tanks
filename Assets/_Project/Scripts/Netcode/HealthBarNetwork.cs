using UnityEngine;

public class HealthBarNetwork : MonoBehaviour
{
    [SerializeField] private Transform foregroundBar;
    [SerializeField] private Transform delayedBarDecreasing;

    public HealthNetwork health;

    public void SetHealth(HealthNetwork health)
    {
        this.health = health;
        health.AddOnHitListener(UpdateBar);
    }

    private void UpdateBar(float value)
    {
        foregroundBar.localScale = new Vector3(value, 1f);
        delayedBarDecreasing.localScale = new Vector3(value, 1f);
    }
}