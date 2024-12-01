using MoreMountains.Feedbacks;
using UnityEngine;

public class FeedbacksManager : Singleton<FeedbacksManager>
{
    [SerializeField] private MMFeedbacks countdownFeedbacks;
    [SerializeField] private MMFeedbacks countdownEndFeedbacks;
    [SerializeField] private MMFeedbacks countdownSpawnFeedbacks;
    [SerializeField] private MMFeedbacks scoreFeedbacks;
    public void PlayCountdownFeedbacks() => countdownFeedbacks?.PlayFeedbacks();
    public void PlayCountdownEndFeedbacks() => countdownEndFeedbacks?.PlayFeedbacks();
    public void PlayCountdownSpawnFeedbacks() => countdownSpawnFeedbacks?.PlayFeedbacks();
    public void PlayScoreFeedbacks() => scoreFeedbacks?.PlayFeedbacks();
}
