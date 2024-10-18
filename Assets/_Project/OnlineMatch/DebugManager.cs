using System;
using TMPro;
using UnityEngine;

public class DebugManager : Singleton<DebugManager>
{
    public TMP_InputField gameTimeInput;
    public TMP_InputField invulnerableTimeInput;
    public TMP_InputField respawnTimeInput;

    private void Start()
    {
        gameTimeInput.onValueChanged.AddListener(OnGameTimeInput);
        invulnerableTimeInput.onValueChanged.AddListener(OnInvulnerableTimeInput);
        respawnTimeInput.onValueChanged.AddListener(OnRespawnTimeInput);
    }

    private void OnRespawnTimeInput(string arg0)
    {
        if (int.TryParse(arg0, out var result))
        {
            GameManager.Instance.respawnTime = result;
        }
    }

    private void OnInvulnerableTimeInput(string arg0)
    {
        if (int.TryParse(arg0, out var result))
        {
            GameManager.Instance.invulnerableTime = result;
        }
    }

    private void OnGameTimeInput(string arg0)
    {
        if (int.TryParse(arg0, out var result))
        {
            GameManager.Instance.gameTime = result;
        }
    }
}
