using UnityEngine;
using UnityEngine.UI;

public class TriggerGameModeEventButton : MonoBehaviour
{
    [SerializeField] private GameModeType eventType;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        GameModeEvent.Trigger(eventType);
    }
}
