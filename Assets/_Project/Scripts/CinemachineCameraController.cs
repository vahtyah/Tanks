using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineCameraController : MonoBehaviour
{
    [SerializeField] Camera cam;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineConfiner confiner;

    public virtual bool FollowsPlayer { get; set; }
    public bool FollowsAPlayer = true;
    public Character TargetCharacter;
    public Collider BoundsCollider;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        confiner = GetComponent<CinemachineConfiner>();
        confiner.m_BoundingVolume = BoundsCollider;
    }

    public virtual void SetTarget(Character character) { TargetCharacter = character; }

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
}