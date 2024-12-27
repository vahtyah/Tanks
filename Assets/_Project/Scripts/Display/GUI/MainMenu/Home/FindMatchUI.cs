using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindMatchUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private FindMatchSupport findMatchSupport;

    [SerializeField] private GameModeSelector gameModeSelector;
    [SerializeField] private GameMapSelector gameMapSelector;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Image readyTimerImage;


    private Animator animator;
    private bool isFindingMatch;
    private Timer matchTimer;
    private Timer readyTimer;

    public bool IsFindingMatch => isFindingMatch;
    public Timer MatchTimer => matchTimer;

    private string startMatch = "FindMatchPanelIn";
    private string cancelMatch = "FindMatchPanelOut";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        button.onClick.AddListener(OnButtonClicked);
        findMatchSupport.AddListenerCancelMatch(OnButtonClicked);
        matchTimer = Timer.Register(new StopWatch())
            .OnTimeRemaining(SetTimerText)
            .OnTimeRemaining(findMatchSupport.SetTimerText);
    }

    void SetTimerText(float time)
    {
        string minutes = Mathf.Floor(time / 60).ToString("00");
        string seconds = (time % 60).ToString("00");
        timerText.text = $"{minutes}:{seconds}";
    }

    private void OnEnable()
    {
        if (!isFindingMatch && animator.isActiveAndEnabled)
        {
            animator.Play("Default");
        }
    }

    public void OnButtonClicked()
    {
        if (isFindingMatch)
        {
            PunManager.Instance.CancelFindMatch();
            if (animator.isActiveAndEnabled)
                animator.Play(cancelMatch);
            buttonText.text = "Find Match";
            matchTimer.Cancel();
        }
        else
        {
            PunManager.Instance.FindMatch(gameModeSelector.GetOption(), gameMapSelector.GetOption());
            if (animator.isActiveAndEnabled)
                animator.Play(startMatch);
            buttonText.text = "Cancel";
            matchTimer.Start();
        }

        isFindingMatch = !isFindingMatch;
        findMatchSupport.IsFindingMatch = isFindingMatch;
    }
}