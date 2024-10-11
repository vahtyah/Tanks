using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Test : MonoBehaviour
{
    public float Gravity = 40f;
    public float GroundedRaycastLength = 5f;
    public float MinimumGroundedDistance = 0.2f;
    public float GroundNormalHeightThreshold = 0.2f;
    public float Deceleration = 10f;
    public float Acceleration1 = 10f;
    public float IdleThreshold = 0.05f;
    public float ContextSpeedMultiplier = 1;
    public float RotateToFaceMovementDirectionSpeed = 10f;
    public GameObject MovementRotatingModel;

    private Rigidbody _rigidbody;
    private Vector3 _newVelocity, _idealVelocity, GroundNormal = Vector3.zero;
    private Vector3 _lastGroundNormal = Vector3.zero, _lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);
    private Vector3 _currentDirection, CurrentMovement;
    Vector2 _normalizedInput, _lerpedInput = Vector2.zero;
    float _acceleration = 0f;
    Vector3 _movementVector;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false; // Tắt gravity của Rigidbody, tự xử lý trong code
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // Đảm bảo không xoay nhân vật không mong muốn
        _currentDirection = transform.forward;
    }

    private void FixedUpdate()
    {
        ProcessUpdate();
        SetMovement();
    }
    
    private void ProcessUpdate()
    {
        AddInput();
        AddGravity();
        MoveRigidbody();
        HandleGroundDetection();
        DetermineDirection();
        RotateToFaceMovementDirection();
    }

    void SetMovement()
    {
        Vector2 currentInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _normalizedInput = currentInput.normalized;

        if (_normalizedInput.magnitude == 0)
        {
            _acceleration = Mathf.Lerp(_acceleration, 0f, Deceleration * Time.deltaTime);
            _lerpedInput = Vector2.Lerp(_lerpedInput, _lerpedInput * _acceleration, Time.deltaTime * Deceleration);
        }
        else
        {
            _acceleration = Mathf.Lerp(_acceleration, 1f, Acceleration1 * Time.deltaTime);
            _lerpedInput = Vector2.ClampMagnitude(_normalizedInput, _acceleration);
        }

        _movementVector = new Vector3(_lerpedInput.x, 0f, _lerpedInput.y) * 6;

        if (_movementVector.magnitude > 6 * ContextSpeedMultiplier)
            _movementVector = Vector3.ClampMagnitude(_movementVector, 6);

        if (currentInput.magnitude <= IdleThreshold && CurrentMovement.magnitude < IdleThreshold)
            _movementVector = Vector3.zero;

        CurrentMovement = _movementVector;
    }

    protected void HandleGroundDetection()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, GroundedRaycastLength))
        {
            if (hit.distance < MinimumGroundedDistance && hit.normal.y > GroundNormal.y)
            {
                if (Mathf.Abs(hit.point.y - _lastHitPoint.y) < GroundNormalHeightThreshold)
                    GroundNormal = hit.normal;
                _lastHitPoint = hit.point;
            }
        }
    }

    private void AddInput()
    {
        _idealVelocity = CurrentMovement;
        Vector3 sideways = Vector3.Cross(Vector3.up, _idealVelocity);
        _idealVelocity = Vector3.Cross(sideways, GroundNormal).normalized * _idealVelocity.magnitude;
        _newVelocity = _idealVelocity;
    }

    void AddGravity()
    {
        _newVelocity.y = Mathf.Min(0, _newVelocity.y) - Gravity * Time.deltaTime;
    }

    void MoveRigidbody()
    {
        // Vector3 targetVelocity = _newVelocity * Time.fixedDeltaTime;
        // _rigidbody.MovePosition(_rigidbody.position + targetVelocity); 
        _rigidbody.velocity = _newVelocity;
    }

    void DetermineDirection()
    {
        if (CurrentMovement.magnitude > 0f)
            _currentDirection = CurrentMovement.normalized;
    }

    void RotateToFaceMovementDirection()
    {
        if (_currentDirection.magnitude >= 0.5f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_currentDirection);
            MovementRotatingModel.transform.rotation = Quaternion.Slerp(MovementRotatingModel.transform.rotation, targetRotation, Time.deltaTime * RotateToFaceMovementDirectionSpeed);
        }
    }
}
