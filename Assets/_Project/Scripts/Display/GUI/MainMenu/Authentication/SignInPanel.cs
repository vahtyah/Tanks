using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignInPanel : MonoBehaviour, IEventListener<AuthenticationEvent>
{
    [SerializeField] private CustomInputField displayNameInputField;
    [SerializeField] private CustomInputField emailInputField;
    [SerializeField] private CustomInputField passwordInputField;

    [SerializeField] private Button signInButton;
    [SerializeField] private TextMeshProUGUI errorText;

    private void Awake()
    {
        signInButton.onClick.AddListener(OnSignInButtonClick);
    }

    private void OnSignInButtonClick()
    {
        string displayName = displayNameInputField.Text;
        string email = emailInputField.Text;
        string password = passwordInputField.Text;

        if (string.IsNullOrEmpty(displayName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            AuthenticationEvent.Trigger(AuthenticationEventType.RegisterFailed,
                errorMessage: "All fields are required.");
            return;
        }

        AuthenticationRequest.Trigger(AuthenticationRequestType.Register, email, password, displayName);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (displayNameInputField.IsFocused)
            {
                displayNameInputField.Deselect();
                emailInputField.Select();
            }
            else if (emailInputField.IsFocused)
            {
                passwordInputField.Select();
            }
            else if (passwordInputField.IsFocused)
            {
                displayNameInputField.Select();
            }
        }
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);

        if (visible)
        {
            displayNameInputField.Text = string.Empty;
            emailInputField.Text = string.Empty;
            passwordInputField.Text = string.Empty;
        }
    }

    public void OnEvent(AuthenticationEvent e)
    {
        switch (e.EventType)
        {
            case AuthenticationEventType.RegisterFailed:
                errorText.text = e.ErrorMessage;
                break;
            default:
                break;
        }
    }

    private void OnEnable()
    {
        this.StartListening();
    }

    private void OnDisable()
    {
        this.StopListening();
    }
}