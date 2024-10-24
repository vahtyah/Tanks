using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainPanelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private String text;
    [SerializeField] private Sprite icon;

    [SerializeField] private Animator buttonAnimator;
    [SerializeField] private TextMeshProUGUI normalText;
    [SerializeField] private TextMeshProUGUI highlightedText;
    [SerializeField] private TextMeshProUGUI pressedText;

    [SerializeField] private Image normalIcon;
    [SerializeField] private Image highlightedIcon;
    [SerializeField] private Image pressedIcon;

    private void Start() { Initialize(); }

    private void Initialize()
    {
        if (buttonAnimator == null) buttonAnimator = GetComponent<Animator>();
        normalText.text = text;
        highlightedText.text = text;
        pressedText.text = text;

        if (icon == null) return;
        normalIcon.sprite = icon;
        highlightedIcon.sprite = icon;
        pressedIcon.sprite = icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
#if !UNITY_ANDROID && !UNITY_IOS
        if (!buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal To Pressed"))
            buttonAnimator.Play("Dissolve To Normal");
#endif
    }

    public void OnPointerExit(PointerEventData eventData)
    {
#if !UNITY_ANDROID && !UNITY_IOS
        if (!buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal To Pressed"))
            buttonAnimator.Play("Normal To Dissolve");
#endif
    }

    private void OnValidate()
    {
        Initialize(); 
        gameObject.name = text;
    }
}