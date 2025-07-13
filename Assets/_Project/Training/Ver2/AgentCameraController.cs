using System;
using UnityEngine;

public class AgentCameraController : Singleton<AgentCameraController>
{
    public GameObject TargetCamera;
    public GameObject GroupCamera;

    protected override void Awake()
    {
        base.Awake();
        TargetCamera.SetActive(true);
        GroupCamera.SetActive(false);
    }

    public void SwitchToTargetCamera(bool isTargetCamera)
    {
        TargetCamera.SetActive(isTargetCamera);
        GroupCamera.SetActive(!isTargetCamera);
    }
}