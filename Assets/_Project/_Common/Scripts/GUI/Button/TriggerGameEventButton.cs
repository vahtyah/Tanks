using UnityEngine;
using UnityEngine.UI;

public class TriggerGameEventButton : MonoBehaviour
{
    [SerializeField] private GameEventType eventType;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        GameEvent.Trigger(eventType);
    }
}
