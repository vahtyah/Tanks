using UnityEngine;

public class CharacterOrientation : CharacterAbility
{
    private Vector3 currentDirection;
    private Vector3 _lastMovement;
    private Quaternion _tmpRotation;
    
    public override void FixedProcessAbility()
    {
        base.FixedProcessAbility();
        RotateToFaceMovementDirection();
    }

    void RotateToFaceMovementDirection()
    {
        currentDirection = controller.CurrentDirection.normalized;
        if (currentDirection.sqrMagnitude >= 0.25f)
        {
            _lastMovement = currentDirection;
        }

        if (_lastMovement != Vector3.zero)
        {
            _tmpRotation = Quaternion.LookRotation(_lastMovement);
            character.transform.rotation = Quaternion.Slerp(transform.rotation, _tmpRotation, Time.deltaTime * 10);
        }
    }
}
