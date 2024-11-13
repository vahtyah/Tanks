using System;
using Photon.Pun;
using UnityEngine;

public class CharacterOrientation : CharacterAbility, IPunObservable
{
    private Vector3 currentDirection;
    private Vector3 lastMovementDirection;
    private Quaternion modelRotation;
    private Quaternion weaponRotation;

    [SerializeField] private bool shouldRotateWeapon;
    private GameObject weaponModel;

    private CharacterMovement movement;

    protected override void PreInitialize()
    {
        base.PreInitialize();
        movement = GetComponent<CharacterMovement>();
        weaponModel = Character.Model.Turret;
    }
    
    public override void ProcessAbility()
    {
        RotateToFaceWeaponDirection();
        RotateToFaceMovementDirection();
        ApplyRotations();
    }

    void RotateToFaceMovementDirection()
    {
        currentDirection = movement.direction.normalized;
        if (currentDirection.sqrMagnitude >= .5f)
        {
            lastMovementDirection = currentDirection;
        }

        if (lastMovementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lastMovementDirection);
            modelRotation = Quaternion.Slerp(Character.transform.rotation, targetRotation, Time.deltaTime * 5);
        }
    }

    private void RotateToFaceWeaponDirection()
    {
        if (!shouldRotateWeapon) return;
            Vector3 aimDirection = Controller.GetAimDirection();
        if (aimDirection != Vector3.zero)
        {
            Quaternion targetWeaponRotation = Quaternion.LookRotation(aimDirection);
            weaponRotation = Quaternion.Slerp(weaponModel.transform.rotation, targetWeaponRotation, Time.deltaTime * 5);
        }
    }

    private void ApplyRotations()
    {
        Character.transform.rotation = modelRotation;
        weaponModel.transform.rotation = weaponRotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Character.transform.rotation);
            stream.SendNext(weaponModel.transform.rotation);
        }
        else
        {
            modelRotation = (Quaternion)stream.ReceiveNext();
            weaponRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    public override void LagCompensation()
    {
        Character.transform.rotation =
            Quaternion.RotateTowards(Character.transform.rotation, modelRotation, Time.deltaTime * 5);
        weaponModel.transform.rotation =
            Quaternion.RotateTowards(weaponModel.transform.rotation, weaponRotation, Time.deltaTime * 5);
    }

    private void Update()
    {
        if (!PhotonView.IsMine)
        {
            LagCompensation();
        }
    }
}