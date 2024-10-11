using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float Gravity = 40f;
    public LayerMask ObstaclesLayerMask = 1 << 8;
    protected Vector3 CurrentDirection;
    public float GroundedRaycastLength = 5f;

    public float MinimumGroundedDistance = 0.2f;

    public float GroundNormalHeightThreshold = 0.2f;

    protected Transform _transform;
    protected CharacterController _characterController;
    protected Vector3 _lastGroundNormal = Vector3.zero;

    // char movement
    protected Vector3 _frameVelocity = Vector3.zero;
    protected Vector3 _hitPoint = Vector3.zero;
    protected Vector3 _lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);

    // velocity
    protected Vector3 _newVelocity;
    protected Vector3 _motion;
    protected Vector3 _idealVelocity;
    protected Vector3 _horizontalVelocityDelta;
    protected float _stickyOffset = 0f;


    // collision detection

    protected float _smallestDistance = Single.MaxValue;
    protected float _longestDistance = Single.MinValue;

    protected RaycastHit _smallestRaycast;
    protected RaycastHit _emptyRaycast = new();
    protected Vector3 _downRaycastsOffset;

    protected RaycastHit _raycastDownHit;
    protected Vector3 _raycastDownDirection = Vector3.down;

    public Vector3 Velocity;

    public Vector3 GroundNormal = Vector3.zero;
    
    Vector2 currentInput;
    Vector2 _normalizedInput;
    protected float _acceleration = 0f;
    protected Vector2 _lerpedInput = Vector2.zero;
    public float Deceleration = 10f;
    public float Acceleration1 = 10f;
    protected Vector3 _movementVector;
    public float IdleThreshold = 0.05f;
    private float _movementSpeedMultiplier = 1;

    public float ContextSpeedMultiplier = 1;
    
    public Vector3 CurrentMovement;


    public virtual float MovementSpeedMaxMultiplier { get; set; } = float.MaxValue;

    public float MovementSpeedMultiplier
    {
        get => Mathf.Min(_movementSpeedMultiplier, MovementSpeedMaxMultiplier);
        set => _movementSpeedMultiplier = value;
    }


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

    void SetMovement()
    {
        currentInput.x = Input.GetAxis("Horizontal");
        currentInput.y = Input.GetAxis("Vertical");

        _normalizedInput = currentInput.normalized;
        if (_normalizedInput.magnitude == 0 || Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical"))
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
        
        //6 thay doi khi ban dan
        
        if (_movementVector.magnitude > 6 * ContextSpeedMultiplier * MovementSpeedMultiplier)
        {
            _movementVector = Vector3.ClampMagnitude(_movementVector, 6);
        }

        if ((currentInput.magnitude <= IdleThreshold) && (CurrentMovement.magnitude < IdleThreshold))
        {
            _movementVector = Vector3.zero;
        }
        CurrentMovement = _movementVector;

        Debug.Log("CurrentMovement = " + CurrentMovement);
    }

    float AdjustDistance(float distance)
    {
        float adjustedDistance = distance - _characterController.height / 2f -
                                 _characterController.skinWidth;
        return adjustedDistance;
    }

    protected void HandleGroundDetection()
    {
        _smallestDistance = Single.MaxValue;
        _longestDistance = Single.MinValue;
        _smallestRaycast = _emptyRaycast;

        // we cast 4 rays downwards to get ground normal
        float offset = _characterController.radius;

        _downRaycastsOffset.x = 0f;
        _downRaycastsOffset.y = 0f;
        _downRaycastsOffset.z = 0f;
        CastRayDownwards();
        _downRaycastsOffset.x = -offset;
        _downRaycastsOffset.y = offset;
        _downRaycastsOffset.z = 0f;
        CastRayDownwards();
        _downRaycastsOffset.x = 0f;
        _downRaycastsOffset.y = offset;
        _downRaycastsOffset.z = -offset;
        CastRayDownwards();
        _downRaycastsOffset.x = offset;
        _downRaycastsOffset.y = offset;
        _downRaycastsOffset.z = 0f;
        CastRayDownwards();
        _downRaycastsOffset.x = 0f;
        _downRaycastsOffset.y = offset;
        _downRaycastsOffset.z = offset;
        CastRayDownwards();

        // we handle our shortest ray
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
                _frameVelocity.x = _frameVelocity.y = _frameVelocity.z = 0f;
            }
        }
    }

    void CastRayDownwards()
    {
        if (_smallestDistance <= MinimumGroundedDistance)
        {
            return;
        }

        Physics.Raycast(this._transform.position + _characterController.center + _downRaycastsOffset,
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

    private void AddInput()
    {
        _idealVelocity = CurrentMovement;
        _idealVelocity += _frameVelocity;
        _idealVelocity.y = 0;
        Vector3 sideways = Vector3.Cross(Vector3.up, _idealVelocity);
        _idealVelocity = Vector3.Cross(sideways, GroundNormal).normalized * _idealVelocity.magnitude;
        _newVelocity = _idealVelocity;
        _newVelocity.y = Mathf.Min(_newVelocity.y, 0);
    }

    void AddGravity()
    {
        _newVelocity.y = Mathf.Min(0, _newVelocity.y) - Gravity * Time.deltaTime;
    }

    void ComputeVelocityDelta()
    {
        _motion = _newVelocity * Time.deltaTime;
        _horizontalVelocityDelta.x = _motion.x;
        _horizontalVelocityDelta.y = 0f;
        _horizontalVelocityDelta.z = _motion.z;
        _stickyOffset = Mathf.Max(_characterController.stepOffset, _horizontalVelocityDelta.magnitude);
        _motion -= _stickyOffset * Vector3.up;
    }

    protected void MoveCharacterController()
    {
        GroundNormal.x = GroundNormal.y = GroundNormal.z = 0f;
        Debug.Log("_motion = " + _motion);
        _characterController.Move(_motion); // controller move

        _lastHitPoint = _hitPoint;
        _lastGroundNormal = GroundNormal;
    }

    void DetermineDirection()
    {
        if (CurrentMovement.magnitude > 0f)
        {
            CurrentDirection = CurrentMovement.normalized;
        }
    }

    protected Vector3 _currentDirection;
    public float AbsoluteThresholdMovement = 0.5f;
    protected Vector3 _lastMovement = Vector3.zero;
    protected Quaternion _tmpRotation;
    protected Quaternion _newMovementQuaternion;
    public GameObject MovementRotatingModel;
    public float RotateToFaceMovementDirectionSpeed = 10f;

    void RotateToFaceMovementDirection()
    {
        _currentDirection = CurrentDirection;

        if (_currentDirection.normalized.magnitude >= AbsoluteThresholdMovement)
        {
            _lastMovement = _currentDirection;
        }

        if (_lastMovement != Vector3.zero)
        {
            _tmpRotation = Quaternion.LookRotation(_lastMovement);
            _newMovementQuaternion = Quaternion.Slerp(MovementRotatingModel.transform.rotation, _tmpRotation,
                Time.deltaTime * RotateToFaceMovementDirectionSpeed);
        }
        MovementRotatingModel.transform.rotation = _newMovementQuaternion;
    }
}