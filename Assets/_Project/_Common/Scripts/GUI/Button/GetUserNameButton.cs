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
            ((LevelManagerBotMatch)LevelManager.Instance).Username = inputField.text;
            GameEvent.Trigger(GameEventType.GameStart);
        }
        else
        {
            inputField.placeholder.color = Color.red;
        }
    }
}
