using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour, IEventListener<AuthenticationEvent>
{
    [SerializeField] private CustomInputField emailInputField;
    [SerializeField] private CustomInputField passwordInputField;

    [SerializeField] private Button loginButton;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Toggle rememberMeToggle;
    
    private void Awake()
    {
        loginButton.onClick.AddListener(OnLoginButtonClick);
    }

    private void OnLoginButtonClick()
    {
        string email = emailInputField.Text;
        string password = passwordInputField.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            AuthenticationEvent.Trigger(AuthenticationEventType.LoginFailed, errorMessage: "Email and password cannot be empty.");
            return;
        }
        
        PlayerPrefs.SetInt(GlobalString.REMEMBER_ME, rememberMeToggle.isOn ? 1 : 0);
        AuthenticationRequest.Trigger(AuthenticationRequestType.Login, email, password);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
        
        if (visible)
        {
            emailInputField.Text = string.Empty;
            passwordInputField.Text = string.Empty;
        }
    }

    public void OnEvent(AuthenticationEvent e)
    {
        switch (e.EventType)
        {
            case AuthenticationEventType.LoginFailed:
                errorText.text = e.ErrorMessage;
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (emailInputField.IsFocused)
            {
                emailInputField.Deselect();
                passwordInputField.Select();
            }
            else if (passwordInputField.IsFocused)
            {
                passwordInputField.Deselect();
                emailInputField.Select();
            }
        }
    }

    private void OnEnable()
    {
        errorText.text = string.Empty;
        // emailInputField.Text = string.Empty;
        // passwordInputField.Text = string.Empty;
        this.StartListening();
    }
    
    private void OnDisable()
    {
        this.StopListening();
    }
}