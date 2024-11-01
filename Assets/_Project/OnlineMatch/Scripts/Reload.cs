using UnityEngine;

public class Reload : MonoBehaviour, IEventListener<GameEvent>
{
    [SerializeField] private Transform foregroundBar;
    private Weapon weapon;

    private void UpdateBar(float value) { foregroundBar.localScale = new Vector3(value, 1f); }

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GameRunning:
                weapon = LevelManager.Instance.GetPlayer("1").EquippedWeapon;
                if (weapon != null)
                    weapon.RegisterOnReloadListener(UpdateBar);
                else
                    Debug.Log("Weapon is null");
                break;
        }
    }

    private void OnEnable() { this.StartListening<GameEvent>(); }

    private void OnDisable() { this.StopListening<GameEvent>(); }
}