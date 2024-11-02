using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class CharacterCaptureFlag : CharacterAbility
{
    [SerializeField] private GameObject flagDisplay;
    [SerializeField] private Renderer flagRenderer;
    [SerializeField] private float captureTime = 2f;

    [ShowInInspector, TitleGroup("Debugs")]
    private Flag currentFlag;

    [ShowInInspector, TitleGroup("Debugs")]
    private Flag targetFlag;

    [ShowInInspector, TitleGroup("Debugs")]
    private List<Flag> nearbyFlags = new List<Flag>();

    private Timer captureTimer;

    protected override void Initialize()
    {
        captureTimer = Timer.Register(captureTime)
            .OnComplete(CompleteCapture);
    }

    private void OnTriggerEnter(Collider other)
    {
        AddFlagToNearBy(other);
    }

    private void AddFlagToNearBy(Collider other)
    {
        if (other.CompareTag("Flag"))
        {
            Flag flag = other.GetComponent<Flag>();
            if (flag && flag && flag.Team != Character.GetTeam() && !nearbyFlags.Contains(flag))
            {
                nearbyFlags.Add(flag);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RemoveFlagFromNearBy(other);
    }

    private void RemoveFlagFromNearBy(Collider other)
    {
        if (other.CompareTag("Flag"))
        {
            Flag flag = other.GetComponent<Flag>();
            if (flag && flag.Team != Character.GetTeam() && nearbyFlags.Contains(flag))
            {
                nearbyFlags.Remove(flag);
                if (flag == targetFlag)
                    targetFlag = null;
            }
        }
    }

    private void Update()
    {
        if (nearbyFlags.Any() && !captureTimer.IsRunning)
        {
            targetFlag = GetClosestFlag();
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCapture();
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            CancelCapture();
        }
    }

    private void CancelCapture()
    {
        captureTimer.Cancel();
        targetFlag = null;
    }

    private void StartCapture()
    {
        captureTimer.Reset();
    }

    private void CompleteCapture()
    {
        if (!targetFlag) return;
        if (currentFlag)
            ReleaseCapturedFlag();
        CaptureFlag();
    }

    private Flag GetClosestFlag()
    {
        return nearbyFlags.OrderBy(flag => Vector3.Distance(transform.position, flag.transform.position))
            .FirstOrDefault();
    }

    private void CaptureFlag()
    {
        if (PhotonView.IsMine)
            PhotonView.RPC(nameof(RPC_CaptureFlag), RpcTarget.All, targetFlag.Team);
    }

    [PunRPC]
    private void RPC_CaptureFlag(TeamType teamType)
    {
        currentFlag = TeamManager.Instance.GetFlag(teamType);
        if (!currentFlag)
        {
            Debug.LogError("Flag reference is null");
            return;
        }

        currentFlag.Capture(teamType);
        flagDisplay.SetActive(true);
        flagRenderer.material = currentFlag.Rd.material;

        nearbyFlags.Remove(targetFlag);
        targetFlag = null;
    }

    public void ReleaseCapturedFlag()
    {
        if (PhotonView.IsMine)
            PhotonView.RPC(nameof(RPC_ReleaseFlag), RpcTarget.All, transform.position);
    }

    [PunRPC]
    private void RPC_ReleaseFlag(Vector3 position)
    {
        if (!currentFlag) return;
        flagDisplay.SetActive(false);
        currentFlag.Release(position);
    }
}