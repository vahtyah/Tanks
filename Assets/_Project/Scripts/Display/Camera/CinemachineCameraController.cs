using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineCameraController : MonoBehaviour, IEventListener<CharacterEvent>
{
    [SerializeField] Camera cam;
    // [SerializeField] private string playerID = "Player1";
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineConfiner confiner;

    public virtual bool FollowsPlayer { get; set; }
    public bool FollowsAPlayer = true;
    public PlayerCharacter TargetCharacter;
    public Collider BoundsCollider;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        confiner = GetComponent<CinemachineConfiner>();
        confiner.m_BoundingVolume = BoundsCollider;
    }

    public virtual void SetTarget(PlayerCharacter character) { TargetCharacter = character; }

    public virtual void StartFollowing()
    {
        if (!FollowsAPlayer)
        {
            return;
        }

        FollowsPlayer = true;
        virtualCamera.Follow = TargetCharacter.CameraTarget.transform;
        virtualCamera.enabled = true;
    }

    public void SetFollowTarget(Transform target) { virtualCamera.Follow = target; }

    public void SetViewportRect(Rect rect) { cam.rect = rect; }

    public void Hide() { cam.enabled = false; }

    public void OnEvent(CharacterEvent e)
    {
        switch (e.EventType)
        {
            case CharacterEventType.CharacterSpawned:
                var player = e.Character as PlayerCharacter;
                if (player != null) SetFollowTarget(player.CameraTarget.transform);
                break;
        }
    }
    
    private void OnEnable() { this.StartListening(); }
    private void OnDisable() { this.StopListening(); }
}