using System;
using Photon.Pun;
using UnityEngine;

public class CharacterMovement : CharacterAbility, IPunObservable
{
    [SerializeField] private float moveSpeed = 150f;
    public Vector3 direction { get; private set; }
    
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    public override void FixedProcessAbility()
    {
        base.FixedProcessAbility();
        HandleMovement();
    }

    private void HandleMovement()
    {
        HandleInput();
        MoveCharacter();
    }

    protected override void HandleInput()
    {
        base.HandleInput();
        direction = Controller.GetDirection().With(y: 0).normalized * (moveSpeed * Time.fixedDeltaTime);
    }

    private void MoveCharacter() { Controller.Move(direction); }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(Controller.GetRigidbody().velocity);
        }
        else
        {
            networkPosition = (Vector3) stream.ReceiveNext();
            Controller.GetRigidbody().velocity = (Vector3) stream.ReceiveNext();
            
            float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
            networkPosition += Controller.GetRigidbody().velocity * lag;
        }
    }

    public override void FixedLagCompensation()
    {
        transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        if(PhotonView.IsMine) return;
        FixedLagCompensation();
    }
}