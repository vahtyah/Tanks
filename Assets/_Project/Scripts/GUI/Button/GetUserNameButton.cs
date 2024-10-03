using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//TODO: Temporarily removed
public class GetUserNameButton : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (inputField.text.Length > 0)
        {
            DatabaseManager.Instance.SetUserName(inputField.text);
            GUIManagerBotMatch.Instance.SetUserName(inputField.text);
            GameEvent.Trigger(GameEventType.GameStart, null);
        }
        else
        {
            Debug.LogWarning("Username cannot be empty");
            inputField.placeholder.color = Color.red;
        }
    }
}
