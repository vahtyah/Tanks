using MoreMountains.Feedbacks;
using Photon.Pun;
using UnityEngine;

public class CharacterBoosterSkill : CharacterAbility
{
    [SerializeField] private float multiplierSpeed = 0.5f;
    [SerializeField] private MMFeedbacks skillFeedbacks;
    [SerializeField] private float skillDuration = 3f;
    [SerializeField] private float skillCooldown = 2f;
    
    
    
    private Timer skillTimer;
    private Timer cooldownTimer;
    private CharacterMovement movement;
    private float multiplierSpeedStorage;

    protected override void PreInitialize()
    {
        base.PreInitialize();
        movement = GetComponent<CharacterMovement>();
        skillTimer = Timer.Register(skillDuration)
            .OnStart(() =>
            {
                multiplierSpeedStorage = movement.SpeedMultiplier;
                movement.SpeedMultiplier = multiplierSpeed;
                PhotonView.RPC(nameof(RPC_Feedback), RpcTarget.All, true);
            })
            .OnComplete(() =>
            {
                movement.SpeedMultiplier = multiplierSpeedStorage;
                PhotonView.RPC(nameof(RPC_Feedback), RpcTarget.All, false);
                cooldownTimer.ReStart();
            })
            .AutoDestroyWhenOwnerDisappear(this);

        cooldownTimer = Timer.Register(skillCooldown)
            .AutoDestroyWhenOwnerDisappear(this);
    }
    
    public override void ProcessAbility()
    {
        base.ProcessAbility();
        HandleInput();
    }
    
    protected override void HandleInput()
    {
        base.HandleInput();
        
        if (Controller.GetInputSkillDown() && !skillTimer.IsRunning && !cooldownTimer.IsRunning)
        {
            skillTimer.ReStart();
        }
    }
    
    [PunRPC]
    private void RPC_Feedback(bool isPlaying)
    {
        if (isPlaying)
            skillFeedbacks?.PlayFeedbacks();
        else
            skillFeedbacks?.StopFeedbacks();
    }
}