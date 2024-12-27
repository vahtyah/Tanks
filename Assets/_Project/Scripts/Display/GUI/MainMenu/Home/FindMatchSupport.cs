using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindMatchSupport : MonoBehaviour, IEventListener<NavigationEvent>
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button cancelButton;

    private Animator animator;
    private string inAnim = "In";
    private string outAnim = "Out";
    private bool isFindingMatch;

    public bool IsFindingMatch
    {
        get => isFindingMatch;
        set => isFindingMatch = value;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        cancelButton.onClick.AddListener(OnCancelButtonClicked);
    }

    public void AddListenerCancelMatch(Action action)
    {
        cancelButton.onClick.AddListener(() => action());
    }

    private void OnCancelButtonClicked()
    {
        animator.Play(outAnim);
        isFindingMatch = false;
    }

    private void SetAnim(bool value)
    {
        if (value && isFindingMatch)
        {
            animator.Play(inAnim);
        }
        else
        {
            animator.Play(outAnim);
        }
    }

    public void SetTimerText(float time)
    {
        string minutes = Mathf.Floor(time / 60).ToString("00");
        string seconds = (time % 60).ToString("00");
        timerText.text = $"{minutes}:{seconds}";
    }

    public void OnEvent(NavigationEvent e)
    {
        switch (e.NavigationType)
        {
            case NavigationType.Home:
                SetAnim(false);
                break;
            default:
                SetAnim(true);
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