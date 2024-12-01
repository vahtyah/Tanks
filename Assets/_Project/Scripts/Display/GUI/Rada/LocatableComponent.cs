using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public abstract class LocatableComponent : MonoBehaviour
{
    public abstract bool ClampOnRadar { get; set; }
    public abstract bool Detected { get; set; }

    protected virtual void OnEnable()
    {
        LocatableManager.Register(this);
    }

    protected virtual void OnDisable()
    {
        LocatableManager.Unregister(this);
    }

    public abstract LocatableIconComponent CreateIcon(Color color, bool isPlayer = false);
    public abstract void SetDetectedForTeam(Player player, bool detected);
}
