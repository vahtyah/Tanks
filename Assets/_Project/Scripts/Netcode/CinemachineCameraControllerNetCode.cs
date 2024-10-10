using Cinemachine;
using UnityEngine;

public class CinemachineCameraControllerNetCode : Singleton<CinemachineCameraControllerNetCode>
{
    private CinemachineVirtualCamera virtualCamera;

    protected override void Awake()
    {
        base.Awake();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    
    public void SetFollowTarget(Transform target) { virtualCamera.Follow = target; }
}
