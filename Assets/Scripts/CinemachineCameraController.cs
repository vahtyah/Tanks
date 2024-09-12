using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineCameraController : MonoBehaviour
{
    [SerializeField] Camera camera;
    private CinemachineVirtualCamera virtualCamera;
    
    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    
    public void SetFollowTarget(Transform target)
    {
        virtualCamera.Follow = target;
    }
    
    
    public void SetViewportRect(Rect rect)
    {
        camera.rect = rect;
    }
    
    public void Hide()
    {
        camera.enabled = false;
    }
}
