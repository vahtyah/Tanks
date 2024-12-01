using System;
using UnityEngine;

public class Controller2 : MonoBehaviour
{
    // Public variables
    public float Gravity = 40f;
    public LayerMask ObstaclesLayerMask = 1 << 8;
    public float GroundedRaycastLength = 5f;
    public float MinimumGroundedDistance = 1f;
    public float GroundNormalHeightThreshold = 0.1f;
    public float Deceleration = 10f;
    public float Acceleration = 10f;
    public float IdleThreshold = 0.05f;
    public float ContextSpeedMultiplier = 1;
    public GameObject MovementRotatingModel;
    public float RotateToFaceMovementDirectionSpeed = 10f;

    // Public properties
    public virtual float MovementSpeedMaxMultiplier { get; set; } = float.MaxValue;
    public float MovementSpeedMultiplier
    {
        get => Mathf.Min(_movementSpeedMultiplier, MovementSpeedMaxMultiplier);
        set => _movementSpeedMultiplier = value;
    }

    public Vector3 CurrentMovement;
    public Vector3 Velocity;
    public Vector3 GroundNormal = Vector3.zero;

    // Protected variables
    protected Transform _transform;
    protected CharacterController _characterController;
    protected Vector3 CurrentDirection;
    protected Vector3 _newVelocity;
    protected Vector3 _motion;
    protected Vector3 _idealVelocity;
    protected Vector3 _horizontalVelocityDelta;
    protected float _stickyOffset = 0f;
    protected float _acceleration = 0f;
    protected float _movementSpeedMultiplier = 1;

    // Movement related variables
    private Vector2 _currentInput;
    private Vector2 _normalizedInput;
    private Vector2 _lerpedInput = Vector2.zero;
    private Vector3 _movementVector;

    // Ground detection variables
    protected Vector3 _downRaycastsOffset;
    protected float _smallestDistance = Single.MaxValue;
    protected float _longestDistance = Single.MinValue;
    protected RaycastHit _smallestRaycast;
    protected RaycastHit _emptyRaycast = new();
    protected RaycastHit _raycastDownHit;
    protected Vector3 _raycastDownDirection = Vector3.down;
    protected Vector3 _hitPoint = Vector3.zero;
    protected Vector3 _lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);
    protected Vector3 _lastGroundNormal = Vector3.zero;

    // Rotation related variables
    protected Vector3 _currentDirection;
    public float AbsoluteThresholdMovement = 0.5f;
    protected Vector3 _lastMovement = Vector3.zero;
    protected Quaternion _tmpRotation;
    protected Quaternion _newMovementQuaternion;
    public Vector3 ModelDirection;
    public Vector3 ModelAngles;

    protected virtual void Awake()
    {
        _transform = transform;
        CurrentDirection = transform.forward;
        _characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        ProcessUpdate();
        SetMovement();
    }

    private void ProcessUpdate()
    {
        _newVelocity = Velocity;
        AddInput();
        AddGravity();
        ComputeVelocityDelta();
        MoveCharacterController();
        HandleGroundDetection();
        DetermineDirection();
        RotateToFaceMovementDirection();
    }

    private void SetMovement()
    {
        _currentInput.x = Input.GetAxis("Horizontal");
        _currentInput.y = Input.GetAxis("Vertical");

        _normalizedInput = _currentInput.normalized;

        if (_normalizedInput.magnitude == 0)
        {
            _acceleration = Mathf.Lerp(_acceleration, 0f, Deceleration * Time.deltaTime);
            _lerpedInput = Vector2.Lerp(_lerpedInput, _lerpedInput * _acceleration, Time.deltaTime * Deceleration);
        }
        else
        {
            _acceleration = Mathf.Lerp(_acceleration, 1f, Acceleration * Time.deltaTime);
            _lerpedInput = Vector2.ClampMagnitude(_normalizedInput, _acceleration);
        }

        _movementVector = new Vector3(_lerpedInput.x, 0f, _lerpedInput.y) * 6;

        if (_movementVector.magnitude > 6 * ContextSpeedMultiplier * MovementSpeedMultiplier)
        {
            _movementVector = Vector3.ClampMagnitude(_movementVector, 6);
        }

        if (_currentInput.magnitude <= IdleThreshold && CurrentMovement.magnitude < IdleThreshold)
        {
            _movementVector = Vector3.zero;
        }

        CurrentMovement = _movementVector;
    }

    private void AddInput()
    {
        _idealVelocity = CurrentMovement;
        _idealVelocity.y = 0;
        Vector3 sideways = Vector3.Cross(Vector3.up, _idealVelocity);
        _idealVelocity = Vector3.Cross(sideways, GroundNormal).normalized * _idealVelocity.magnitude;
        _newVelocity = _idealVelocity;
        _newVelocity.y = Mathf.Min(_newVelocity.y, 0);
    }

    private void AddGravity()
    {
        _newVelocity.y = Mathf.Min(0, _newVelocity.y) - Gravity * Time.deltaTime;
    }

    private void ComputeVelocityDelta()
    {
        _motion = _newVelocity * Time.deltaTime;
        _horizontalVelocityDelta = new Vector3(_motion.x, 0f, _motion.z);
        _stickyOffset = Mathf.Max(_characterController.stepOffset, _horizontalVelocityDelta.magnitude);
        _motion -= _stickyOffset * Vector3.up;
    }

    protected void MoveCharacterController()
    {
        GroundNormal = Vector3.zero;
        _characterController.Move(_motion);
        _lastHitPoint = _hitPoint;
        _lastGroundNormal = GroundNormal;
    }

    protected void HandleGroundDetection()
    {
        _smallestDistance = Single.MaxValue;
        _longestDistance = Single.MinValue;
        _smallestRaycast = _emptyRaycast;

        float offset = _characterController.radius;

        _downRaycastsOffset = Vector3.zero;
        CastRayDownwards();
        _downRaycastsOffset = new Vector3(-offset, offset, 0f);
        CastRayDownwards();
        _downRaycastsOffset = new Vector3(0f, offset, -offset);
        CastRayDownwards();
        _downRaycastsOffset = new Vector3(offset, offset, 0f);
        CastRayDownwards();
        _downRaycastsOffset = new Vector3(0f, offset, offset);
        CastRayDownwards();

        if (_smallestRaycast.collider != null)
        {
            if (_smallestRaycast.normal.y > 0 && _smallestRaycast.normal.y > GroundNormal.y)
            {
                if ((Mathf.Abs(_smallestRaycast.point.y - _lastHitPoint.y) < GroundNormalHeightThreshold) &&
                    ((_smallestRaycast.point != _lastHitPoint) || (_lastGroundNormal == Vector3.zero)))
                {
                    GroundNormal = _smallestRaycast.normal;
                }
                else
                {
                    GroundNormal = _lastGroundNormal;
                }

                _hitPoint = _smallestRaycast.point;
            }
        }
    }

    private void CastRayDownwards()
    {
        if (_smallestDistance <= MinimumGroundedDistance) return;

        Physics.Raycast(_transform.position + _characterController.center + _downRaycastsOffset,
            _raycastDownDirection, out _raycastDownHit,
            _characterController.height / 2f + GroundedRaycastLength, ObstaclesLayerMask);

        if (_raycastDownHit.collider != null)
        {
            float adjustedDistance = AdjustDistance(_raycastDownHit.distance);

            if (adjustedDistance < _smallestDistance)
            {
                _smallestDistance = adjustedDistance;
                _smallestRaycast = _raycastDownHit;
            }

            if (adjustedDistance > _longestDistance)
            {
                _longestDistance = adjustedDistance;
            }
        }
    }

    private float AdjustDistance(float distance)
    {
        return distance - _characterController.height / 2f - _characterController.skinWidth;
    }

    private void DetermineDirection()
    {
        if (CurrentMovement.magnitude > 0f)
        {
            CurrentDirection = CurrentMovement.normalized;
        }
    }

    private void RotateToFaceMovementDirection()
    {
        _currentDirection = CurrentDirection;

        if (_currentDirection.magnitude >= AbsoluteThresholdMovement)
        {
            _lastMovement = _currentDirection;
        }

        if (_lastMovement != Vector3.zero)
        {
            _tmpRotation = Quaternion.LookRotation(_lastMovement);
            _newMovementQuaternion = Quaternion.Slerp(MovementRotatingModel.transform.rotation, _tmpRotation,
                Time.deltaTime * RotateToFaceMovementDirectionSpeed);
        }

        ModelDirection = MovementRotatingModel.transform.forward.normalized;
        ModelAngles = MovementRotatingModel.transform.eulerAngles;
        MovementRotatingModel.transform.rotation = _newMovementQuaternion;
    }
}
