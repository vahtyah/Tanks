using UnityEngine;

public class CharacterOrientation : CharacterAbility
{
    private Vector3 currentDirection;
    private Vector3 _lastMovement;
    private Quaternion _tmpRotation;
    private Quaternion newModelRotation;

    [SerializeField] private bool shouldRotateWeapon;
    [SerializeField] private GameObject weaponModel;

    private Vector3 rotationDirection;
    private Quaternion tmpWeaponRotation;
    private Quaternion newWeaponRotation;

    private CharacterMovement movement;

    protected override void PreInitialization()
    {
        base.PreInitialization();
        movement = GetComponent<CharacterMovement>();

    }

    public override void FixedProcessAbility()
    {
        base.FixedProcessAbility();
        RotateToFaceWeaponDirection();
        RotateToFaceMovementDirection();
        RotateModel();
    }

    void RotateToFaceMovementDirection()
    {
        currentDirection = movement.direction.normalized;
        if (currentDirection.sqrMagnitude >= .5f)
        {
            _lastMovement = currentDirection;
        }

        if (_lastMovement != Vector3.zero)
        {
            _tmpRotation = Quaternion.LookRotation(_lastMovement);
            newModelRotation = Quaternion.Slerp(character.transform.rotation, _tmpRotation, Time.deltaTime * 5);
        }
    }

    private void RotateToFaceWeaponDirection()
    {
        if (!shouldRotateWeapon) return;
        rotationDirection = controller.GetAimDirection();
        if (rotationDirection != Vector3.zero)
        {
            _tmpRotation = Quaternion.LookRotation(rotationDirection);
            newWeaponRotation = Quaternion.Slerp(weaponModel.transform.rotation, _tmpRotation, Time.deltaTime * 5);
        }
    }

    private void RotateModel()
    {
        character.transform.rotation = newModelRotation;
        if (rotationDirection != Vector3.zero)
            weaponModel.transform.rotation = newWeaponRotation;
    }
}