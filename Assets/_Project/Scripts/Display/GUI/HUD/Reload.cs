using UnityEngine;

public class Reload : MonoBehaviour, IEventListener<CharacterEvent>
{
    [SerializeField] private Transform foregroundBar;
    private Weapon weapon;

    private void UpdateBar(float value) { foregroundBar.localScale = new Vector3(value, 1f); }

    public void OnEvent(CharacterEvent e)
    {
        switch (e.EventType)
        {
            case CharacterEventType.CharacterSpawned:
                weapon = e.Character.EquippedWeapon;
                if (weapon != null)
                    weapon.RegisterOnReloadListener(UpdateBar);
                else
                    UnityEngine.Debug.Log("Weapon is null");
                break;
        }
    }

    private void OnEnable() { this.StartListening(); }

    private void OnDisable() { this.StopListening(); }
}