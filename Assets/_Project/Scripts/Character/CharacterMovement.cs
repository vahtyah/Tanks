using UnityEngine;

public class CharacterMovement : CharacterAbility
{
    [SerializeField] private float moveSpeed = 150f;
    public Vector3 direction { get; private set; }

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
        direction = controller.GetDirection().With(y:0).normalized * (moveSpeed * Time.fixedDeltaTime);
    }

    private void MoveCharacter()
    {
        controller.Move(direction);
    }
}