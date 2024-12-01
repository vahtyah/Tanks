using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(UnPauseOnClick);
    }

    public void UnPauseOnClick()
    {
        GameEvent.Trigger(GameEventType.TogglePause);
    }
}