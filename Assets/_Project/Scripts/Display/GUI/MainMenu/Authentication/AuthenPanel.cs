using System;
using UnityEngine;
using UnityEngine.UI;

public class AuthenPanel : MonoBehaviour, IEventListener<AuthenticationEvent>
{
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;

    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;

    private Animator loginPanelAnimator;
    private Animator registerPanelAnimator;

    string panelInRight = "InRight";
    string panelOutRight = "OutRight";
    string panelInLeft = "InLeft";
    string panelOutLeft = "OutLeft";

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLoginButtonClick);
        registerButton.onClick.AddListener(OnRegisterButtonClick);
        loginPanelAnimator = loginPanel.GetComponent<Animator>();
        registerPanelAnimator = registerPanel.GetComponent<Animator>();
    }

    private void OnRegisterButtonClick()
    {
        loginPanelAnimator.Play(panelOutRight);
        registerPanelAnimator.Play(panelInLeft);
    }

    private void OnLoginButtonClick()
    {
        loginPanelAnimator.Play(panelInRight);
        registerPanelAnimator.Play(panelOutLeft);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void OnEvent(AuthenticationEvent e)
    {
        switch (e.EventType)
        {
            case AuthenticationEventType.RegisterSuccessful:
                NotificationEvent.Trigger("Authentication", "Register successful");
                OnLoginButtonClick();
                break;
        }
    }

    private void OnEnable()
    {
        loginPanelAnimator.Play(panelInLeft);
        this.StartListening();
    }

    private void OnDisable()
    {
        this.StopListening();
    }
}