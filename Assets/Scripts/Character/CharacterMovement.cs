using UnityEngine;

public class CharacterMovement : CharacterAbility
{
    [SerializeField] private float moveSpeed = 150f;
    Vector3 directionInput;

    public override void FixedProcessAbility()
    {
        base.FixedProcessAbility();
        HandleMovement();
    }

    private void HandleMovement()
    {
        HandleInput();
        SetMoveDirection();
    }

    protected override void HandleInput()
    {
        base.HandleInput();
        directionInput = new Vector3(inputManager.GetAxisHorizontal(), 0, inputManager.GetAxisVertical()).normalized *
                         (moveSpeed * Time.deltaTime);
    }

    private void SetMoveDirection() { controller.SetDirection(directionInput); }
}