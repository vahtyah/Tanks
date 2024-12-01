using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraMainMenu : MonoBehaviour
{
    CinemachineVirtualCamera vcam;

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    public void ChangeTarget(Transform target)
    {
        vcam.Follow = target;
        vcam.LookAt = target;
    }
}