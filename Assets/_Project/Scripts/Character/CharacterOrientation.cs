using UnityEngine;

public class CharacterOrientation : CharacterAbility
{
    private Vector3 currentDirection;
    private Vector3 _lastMovement;
    private Quaternion _tmpRotation;
    private Quaternion newModelRotation;

    [SerializeField] private bool shouldRotateWeapon;
    [SerializeField] private GameObject weaponModel;
    [SerializeField] private GameObject model;

    private Vector3 rotationDirection;
    private Quaternion tmpWeaponRotation;
    private Vector3 mouseWorldPosition;
    private Quaternion newWeaponRotation;

    private Plane plane;

    protected override void Initialization()
    {
        base.Initialization();
        if (shouldRotateWeapon)
        {
            // Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        plane = new Plane(Vector3.up, Vector3.zero);
    }

    public override void FixedProcessAbility()
    {
        base.FixedProcessAbility();
        RotateToFaceWeaponDirection();
        RotateToFaceMovementDirection();
        RotateModel();
        plane.SetNormalAndPosition(Vector3.up, transform.position);
    }

    void RotateToFaceMovementDirection()
    {
        currentDirection = controller.CurrentDirection.normalized;
        if (currentDirection.sqrMagnitude >= .5f)
        {
            _lastMovement = currentDirection;
        }

        if (_lastMovement != Vector3.zero)
        {
            _tmpRotation = Quaternion.LookRotation(_lastMovement);
            newModelRotation = Quaternion.Slerp(model.transform.rotation, _tmpRotation, Time.deltaTime * 5);
        }
    }

    private void RotateToFaceWeaponDirection()
    {
        if (!shouldRotateWeapon) return;
        mouseWorldPosition = characterInput.GetMouseWorldPosition(weaponModel.transform.position);
        rotationDirection = new Vector3(mouseWorldPosition.x - weaponModel.transform.position.x, 0,
            mouseWorldPosition.z - weaponModel.transform.position.z);
        if (rotationDirection != Vector3.zero)
        {
            _tmpRotation = Quaternion.LookRotation(rotationDirection);
            newWeaponRotation = Quaternion.Slerp(weaponModel.transform.rotation, _tmpRotation, Time.deltaTime * 5);
        }
    }

    private void RotateModel()
    {
        model.transform.rotation = newModelRotation;
        // if (rotationDirection != Vector3.zero)
            weaponModel.transform.rotation = newWeaponRotation;
    }
}