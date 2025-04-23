using System;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class CharacterMovement : CharacterAbility, IPunObservable
{
    [SerializeField, BoxGroup("Settings")] private float moveSpeed = 150f;
    [Log] private float speedMultiplier = 1f;
    public Vector3 MovementDirection { get; private set; }

    private Vector3 syncedPosition;
    private Quaternion syncedRotation;

    public float SpeedMultiplier
    {
        get => speedMultiplier;
        set => speedMultiplier = Mathf.Clamp(value, 0, 2);
    }

    public override void FixedProcessAbility()
    {
        base.FixedProcessAbility();
        HandleMovement();
    }

    private void HandleMovement()
    {
        HandleInput();
        ApplyMovement();
    }

    protected override void HandleInput()
    {
        base.HandleInput();
        MovementDirection = Controller.GetDirection().With(y: 0).normalized *
                            (moveSpeed * Time.fixedDeltaTime * SpeedMultiplier);
    }

    private void ApplyMovement()
    {
        Controller.Move(MovementDirection);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(Controller.GetRigidbody().velocity);
        }
        else
        {
            syncedPosition = (Vector3)stream.ReceiveNext();
            Controller.GetRigidbody().velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            syncedPosition += Controller.GetRigidbody().velocity * lag;
        }
    }

    public override void FixedLagCompensation()
    {
        transform.position = Vector3.MoveTowards(transform.position, syncedPosition, Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        if (PhotonView.IsMine) return;
        FixedLagCompensation();
    }
}