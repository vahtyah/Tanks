using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class Flag : MonoBehaviour
{
    [SerializeField] private Renderer rd;
    [SerializeField] private ForceFieldController capturingEffect;
    
    public Renderer Rd => rd;
    public TeamType Team { get; private set; }
    public TeamType TeamCaptured { get; private set; } = TeamType.None;
    private Vector3 initialPosition;
    public float returningTime = 5f;
    private Timer returningTimer;
    private Indicator indicator;


    private void Start()
    {
        initialPosition = transform.position;
        returningTimer = Timer.Register(returningTime)
            .OnStart(() =>
            {
                indicator = Indicator.GetIndicator(transform);
                indicator.StartReturningEffect(returningTime);
            })
            .OnTimeRemaining(remaining => indicator.SetCountdownText(remaining))
            .OnComplete(() =>
            {
                indicator.StopReturningEffect();
                Return();
            })
            .AutoDestroyWhenOwnerDisappear(this);
    }

    public void Initialize(TeamType team, Material material)
    {
        Team = team;
        rd.material = material;
    }

    public void Reset()
    {
        TeamCaptured = TeamType.None;
    }

    public void Capture(TeamType team)
    {
        if (TeamCaptured == TeamType.None)
        {
            TeamCaptured = team;
            gameObject.SetActive(false);
        }
    }

    public void Release(Vector3 position)
    {
        TeamCaptured = TeamType.None;
        transform.position = position.With(y: initialPosition.y);
        gameObject.SetActive(true);
        returningTimer.ReStart();
    }

    public void Return()
    {
        TeamCaptured = TeamType.None;
        transform.position = initialPosition;
        gameObject.SetActive(true);
    }
    
    public void StartCapturingEffect()
    {
        capturingEffect?.HandleOpenClose(true);
    }
    
    public void UpdateCapturingEffect(float fillAmount)
    {
        capturingEffect?.OpenCloseProgress(fillAmount);
    }
    public void StopCapturingEffect()
    {
        capturingEffect?.HandleOpenClose(false);
    }
    
    public void ChangeColorCapturingEffect(Color color)
    {
        capturingEffect.SetColor(color);
    }

    public bool IsCaptured => TeamCaptured != TeamType.None && !gameObject.activeSelf;

}