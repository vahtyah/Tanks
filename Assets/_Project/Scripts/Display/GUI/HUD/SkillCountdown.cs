using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCountdown : MonoBehaviour, IEventListener<SkillEvent>
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image countdownImage;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI amountLeftText;
    private Timer countdownTimer;

    private void Start()
    {
        countdownText.gameObject.SetActive(false);
        amountLeftText.gameObject.SetActive(false);
    }

    public void Initialize(float duration, Sprite icon)
    {
        backgroundImage.sprite = icon;
        countdownTimer = Timer.Register(duration)
            .OnStart(() =>
            {
                countdownImage.fillAmount = 0;
                countdownText.gameObject.SetActive(true);
                countdownText.text = duration.ToString("F0");
                amountLeftText.gameObject.SetActive(false);
            })
            .OnRemaining((remaini) => { countdownImage.fillAmount = remaini; })
            .OnTimeRemaining((remaining) => { countdownText.text = remaining.ToString("F0"); })
            .OnComplete(() =>
            {
                countdownImage.fillAmount = 0;
                countdownText.gameObject.SetActive(false);
            }).AutoDestroyWhenOwnerDisappear(this);
    }

    public void StartCountdown()
    {
        countdownTimer.ReStart();
    }

    public void SetAmountLeft(float amount)
    {
        if(amount <= 0)
        {
            amountLeftText.gameObject.SetActive(false);
            return;
        }
        amountLeftText.gameObject.SetActive(true);
        amountLeftText.text = amount.ToString(CultureInfo.InvariantCulture);
    }

    public void OnEvent(SkillEvent e)
    {
        switch (e.WeaponState)
        {
            case WeaponState.Initializing:
                Initialize(e.Param, e.Icon);
                break;

            case WeaponState.Firing:
                SetAmountLeft(e.Param);
                break;

            case WeaponState.Reloading:
                StartCountdown();
                break;
            
            case WeaponState.Ready:
                SetAmountLeft(e.Param);
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