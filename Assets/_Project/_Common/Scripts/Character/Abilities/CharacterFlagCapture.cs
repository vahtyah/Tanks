using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class CharacterFlagCapture : CharacterAbility, IEventListener<CharacterEvent>
{
    [SerializeField, BoxGroup("Settings")] private float captureTime = 2f;
    [SerializeField, BoxGroup("Settings")] private float handInTime = 1f;


    [ShowInInspector, TitleGroup("Debugs")]
    private Flag currentFlag;

    [ShowInInspector, TitleGroup("Debugs")]
    private Flag targetFlag;

    [ShowInInspector, TitleGroup("Debugs")]
    private List<Flag> nearbyFlags = new();

    [Debug]private ForceFieldController flagDisplay;
    [Debug]private Renderer flagRenderer;
    private Timer captureTimer;
    private Timer handInTimer;
    [Debug] private GUIManagerOnlineMatch GUI;
    private Transform teamArea;

    protected override void PreInitialize()
    {
        base.PreInitialize();
        flagDisplay = Character.Model.FlagDisplay;
        flagRenderer = Character.Model.FlagRenderer;
        if (PhotonNetwork.CurrentRoom.GetGameMode() != GameMode.CaptureTheFlag)
            Destroy(this);
    }

    protected override void Initialize()
    {
        PhotonView.RPC(nameof(RPC_Initialize), RpcTarget.All);
        GUI = GUIManagerOnlineMatch.Instance;
        teamArea = EnvironmentManager.Instance.CurrentMap.GetAreaTransform(PhotonView.Owner.GetTeam());
        RegisterTimers();
    }

    private void RegisterTimers()
    {
        if(!Character.PhotonView.IsMine) return;
        captureTimer = Timer.Register(captureTime)
            .OnStart(() =>
            {
                GUI.SetCaptureProgress(0);
                PhotonView.RPC(nameof(RPC_StartTimerCapture), RpcTarget.All);
            })
            .OnProgress(progress =>
            {
                PhotonView.RPC(nameof(RPC_UpdateTimerCapture), RpcTarget.All, progress);
                GUI.SetCaptureProgress(progress);
            })
            .OnComplete(() =>
            {
                PhotonView.RPC(nameof(RPC_StopTimerCapture), RpcTarget.All);
                CompleteCapture();
            })
            .OnDone(() =>
            {
                GUI.SetCaptureProgress(0);
                PhotonView.RPC(nameof(RPC_StopTimerCapture), RpcTarget.All);
            });
        handInTimer = Timer.Register(handInTime)
            .OnComplete(CompleteHandIn);
    }

    [PunRPC]
    private void RPC_StartTimerCapture()
    {
        targetFlag?.ChangeColorCapturingEffect(Team.GetTeamByType(Character.GetTeamType()).TeamColor);
        targetFlag?.StartCapturingEffect();
    }

    [PunRPC]
    private void RPC_StopTimerCapture()
    {
        targetFlag?.StopCapturingEffect();
    }

    [PunRPC]
    private void RPC_UpdateTimerCapture(float progress)
    {
        targetFlag?.UpdateCapturingEffect(progress);
    }

    [PunRPC]
    private void RPC_Initialize()
    {
        flagDisplay.SetColor(Team.GetTeamByType(Character.GetTeamType()).TeamColor);
    }

    private void CompleteHandIn()
    {
        if (!currentFlag) return;
        ReturnFlag();
        Character.AddScore(10);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleFlagProximity(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleFlagProximity(other, false);
    }


    private void OnTriggerStay(Collider other)
    {
        if (!PhotonView.IsMine) return;
        if (other.CompareTag("Flag"))
        {
            PhotonView.RPC(nameof(RPC_SetTargetFlag), RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_SetTargetFlag()
    {
        targetFlag = GetClosestFlag();
    }

    private void HandleFlagProximity(Collider other, bool isEntering)
    {
        if(!PhotonView.IsMine) return;
        if (other.CompareTag("Flag"))
        {
            Flag flag = other.GetComponent<Flag>();
            if (flag != null)
            {
                if (flag.Team != Character.GetTeamType())
                {
                    UpdateNearbyFlags(isEntering, flag);
                }
            }
        }

        if (other.CompareTag("SpawnArea") && teamArea)
        {
            if(teamArea.gameObject == other.gameObject)
                HandleHandInTimer(isEntering);
        }
    }

    private void UpdateNearbyFlags(bool isEntering, Flag flag)
    {
        if (isEntering && !nearbyFlags.Contains(flag))
        {
            nearbyFlags.Add(flag);
        }
        else if (!isEntering && nearbyFlags.Contains(flag))
        {
            nearbyFlags.Remove(flag);
            if (flag == targetFlag)
            {
                UnSetTargetFlag();
            }
        }
    }

    private void HandleHandInTimer(bool isEntering)
    {
        if (isEntering)
        {
            handInTimer.ReStart();
        }
        else
        {
            handInTimer.Cancel();
        }
    }

    private void Update()
    {
        if (!PhotonView.IsMine) return;
        if (nearbyFlags.Any() && !captureTimer.IsRunning)
        {
            if (Input.GetKeyDown(KeyCode.E) && targetFlag)
            {
                StartCapture();
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            CancelCapture();
        }

        if (targetFlag)
        {
            if (targetFlag.IsCaptured)
            {
                nearbyFlags.Remove(targetFlag);
                UnSetTargetFlag();
                return;
            }

            GUI.SetTargetIndicatorActive(true);
            GUI.SetTargetIndicatorPosition(targetFlag.transform.position.Add(y:.5f));
        }
        else
        {
            GUI.SetTargetIndicatorActive(false);
        }
    }

    private void CancelCapture()
    {
        captureTimer.Cancel();
        UnSetTargetFlag();
    }

    private void UnSetTargetFlag()
    {
        targetFlag?.StopCapturingEffect();
        targetFlag = null;
    }

    private void StartCapture()
    {
        captureTimer.ReStart();
    }

    private void CompleteCapture()
    {
        if (!targetFlag) return;
        if (currentFlag)
            ReleaseCapturedFlag(targetFlag.transform.position);
        CaptureFlag();
    }

    private Flag GetClosestFlag()
    {
        // nearbyFlags.RemoveAll(flag => flag == null || flag.gameObject.activeSelf == false);
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
            UnityEngine.Debug.LogError("Flag reference is null");
            return;
        }

        currentFlag.Capture(teamType);
        flagDisplay.HandleOpenClose(true);
        flagRenderer.material = currentFlag.Rd.material;

        nearbyFlags.Remove(targetFlag);
        UnSetTargetFlag();
        var indicator = Indicator.UpdateTarget(currentFlag.transform, transform);

        if (currentFlag.Team == PhotonNetwork.LocalPlayer.GetTeam().TeamType)
            indicator.StartOwnerMovingEffect();
        else
            indicator.StartMovingEffect();
    }

    public void ReleaseCapturedFlag(Vector3 position)
    {
        if (PhotonView.IsMine)
            PhotonView.RPC(nameof(RPC_ReleaseFlag), RpcTarget.All, position);
    }

    [PunRPC]
    private void RPC_ReleaseFlag(Vector3 position)
    {
        if (!currentFlag) return;
        flagDisplay.HandleOpenClose(false);
        var indicator = Indicator.UpdateTarget(transform, currentFlag.transform);
        indicator.StopMovingEffect();
        currentFlag.Release(position);
    }

    private void ReturnFlag()
    {
        if (PhotonView.IsMine)
            PhotonView.RPC(nameof(RPC_ReturnFlag), RpcTarget.All);
    }

    [PunRPC]
    void RPC_ReturnFlag()
    {
        if (!currentFlag) return;
        flagDisplay.HandleOpenClose(false);
        flagDisplay.gameObject.SetActive(false);
        var indicator = Indicator.UpdateTarget(transform, currentFlag.transform);
        indicator.StopMovingEffect();
        currentFlag.Return();
        currentFlag = null;
    }

    public void OnEvent(CharacterEvent e)
    {
        if(!Character.PhotonView.IsMine) return;
        switch (e.EventType)
        {
            case CharacterEventType.CharacterDeath:
                if (currentFlag)
                    ReleaseCapturedFlag(Character.transform.position);
                break;
        }
    }
    
    private void OnEnable()
    {
        this.StartListening();
    }
    
    private void OnDisable()
    {
        this.StopListening();
    }
}