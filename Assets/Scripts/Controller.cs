using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float Gravity = 40f;
    public float CrouchedRaycastLengthMultiplier = 1f;
    public Vector3 Speed;

    [Tooltip("the direction the character is going in")]
    protected Vector3 CurrentDirection;

    public Vector3 InputMoveDirection = Vector3.zero;

    public Vector3 ColliderCenter
    {
        get { return this.transform.position + _characterController.center; }
    }

    public Vector3 ColliderBottom
    {
        get
        {
            return this.transform.position + _characterController.center +
                   Vector3.down * _characterController.bounds.extents.y;
        }
    }

    public Vector3 ColliderTop
    {
        get
        {
            return this.transform.position + _characterController.center +
                   Vector3.up * _characterController.bounds.extents.y;
        }
    }

    // the obstacle left to this controller (only updated if DetectObstacles is called)
    public virtual GameObject DetectedObstacleLeft { get; set; }

    // the obstacle right to this controller (only updated if DetectObstacles is called)
    public virtual GameObject DetectedObstacleRight { get; set; }

    // the obstacle up to this controller (only updated if DetectObstacles is called)
    public virtual GameObject DetectedObstacleUp { get; set; }

    // the obstacle down to this controller (only updated if DetectObstacles is called)
    public virtual GameObject DetectedObstacleDown { get; set; }

    // true if an obstacle was detected in any of the cardinal directions
    public virtual bool CollidingWithCardinalObstacle { get; set; }

    protected Vector3 _positionLastFrame;
    protected Vector3 _speedComputation;
    protected bool _groundedLastFrame;
    protected Vector3 _impact;
    protected const float _smallValue = 0.0001f;

    public enum GroundedComputationModes
    {
        Simple,
        Advanced
    }

    public Vector3 AddedForce;

    /// the layer to consider as obstacles (will prevent movement)
    [Tooltip("the layer to consider as obstacles (will prevent movement)")]
    public LayerMask ObstaclesLayerMask = 1 << 8;

    /// the length of the raycasts to cast downwards
    [Tooltip("the length of the raycasts to cast downwards")]
    public float GroundedRaycastLength = 5f;

    /// the distance to the ground beyond which the character isn't considered grounded anymore
    [Tooltip("the distance to the ground beyond which the character isn't considered grounded anymore")]
    public float MinimumGroundedDistance = 0.2f;

    /// the selected modes to compute grounded state. Simple should only be used if your ground is even and flat
    [Tooltip(
        "the selected modes to compute grounded state. Simple should only be used if your ground is even and flat")]
    public GroundedComputationModes GroundedComputationMode = GroundedComputationModes.Advanced;

    /// a threshold against which to check when going over steps. Adjust that value if your character has issues going over small steps
    [Tooltip(
        "a threshold against which to check when going over steps. Adjust that value if your character has issues going over small steps")]
    public float GroundNormalHeightThreshold = 0.2f;

    /// the speed at which external forces get lerped to zero
    [Tooltip("the speed at which external forces get lerped to zero")]
    public float ImpactFalloff = 5f;

    protected Transform _transform;
    protected Rigidbody _rigidBody;
    protected Collider _collider;
    protected CharacterController _characterController;
    protected float _originalColliderHeight;
    protected Vector3 _originalColliderCenter;
    protected Vector3 _originalSizeRaycastOrigin;
    protected Vector3 _lastGroundNormal = Vector3.zero;
    protected WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
    protected bool _detached = false;

    // moving platforms
    protected Transform _movingPlatformHitCollider;
    protected Transform _movingPlatformCurrentHitCollider;
    protected Vector3 _movingPlatformCurrentHitColliderLocal;
    protected Vector3 _movingPlatformCurrentGlobalPoint;
    protected Quaternion _movingPlatformLocalRotation;
    protected Quaternion _movingPlatformGlobalRotation;
    protected Matrix4x4 _lastMovingPlatformMatrix;
    protected Vector3 _movingPlatformVelocity;
    protected bool _newMovingPlatform;

    // char movement
    protected CollisionFlags _collisionFlags;
    protected Vector3 _frameVelocity = Vector3.zero;
    protected Vector3 _hitPoint = Vector3.zero;
    protected Vector3 _lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);

    // velocity
    protected Vector3 _newVelocity;
    protected Vector3 _lastHorizontalVelocity;
    protected Vector3 _newHorizontalVelocity;
    protected Vector3 _motion;
    protected Vector3 _idealVelocity;
    protected Vector3 _idealDirection;
    protected Vector3 _horizontalVelocityDelta;
    protected float _stickyOffset = 0f;

    // move position
    protected RaycastHit _movePositionHit;
    protected Vector3 _capsulePoint1;
    protected Vector3 _capsulePoint2;
    protected Vector3 _movePositionDirection;
    protected float _movePositionDistance;

    // collision detection
    protected RaycastHit _cardinalRaycast;

    protected float _smallestDistance = Single.MaxValue;
    protected float _longestDistance = Single.MinValue;

    protected RaycastHit _smallestRaycast;
    protected RaycastHit _emptyRaycast = new RaycastHit();
    protected Vector3 _downRaycastsOffset;

    protected Vector3 _moveWithPlatformMoveDistance;
    protected Vector3 _moveWithPlatformGlobalPoint;
    protected Quaternion _moveWithPlatformGlobalRotation;
    protected Quaternion _moveWithPlatformRotationDiff;
    protected RaycastHit _raycastDownHit;
    protected Vector3 _raycastDownDirection = Vector3.down;
    protected RaycastHit _canGoBackHeadCheck;
    protected bool _tooSteepLastFrame;

    public Vector3 VelocityLastFrame;
    public Vector3 Velocity;

    public Vector3 GroundNormal = Vector3.zero;
    public Vector3 Acceleration;

    protected Vector2 _primaryMovement = Vector2.zero;
    protected Vector2 _secondaryMovement = Vector2.zero;

    protected virtual void Awake()
    {
        _transform = transform;
        CurrentDirection = transform.forward;
        _characterController = GetComponent<CharacterController>();
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _originalColliderHeight = _characterController.height;
        _originalColliderCenter = _characterController.center;
    }

    private void LateUpdate() { VelocityLastFrame = Velocity; }

    private void FixedUpdate()
    {
        GetMovingPlatformVelocity();
        ProcessUpdate();
        SetMovement();
    }

    protected void HandleAdvancedGroundDetection()
    {
        if (GroundedComputationMode != GroundedComputationModes.Advanced)
        {
            return;
        }

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
            float adjustedDistance = AdjustDistance(_smallestRaycast.distance);

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

                _movingPlatformHitCollider = _smallestRaycast.collider.transform;
                _hitPoint = _smallestRaycast.point;
                _frameVelocity.x = _frameVelocity.y = _frameVelocity.z = 0f;
            }
        }
    }

    float AdjustDistance(float distance)
    {
        float adjustedDistance = distance - _characterController.height / 2f -
                                 _characterController.skinWidth;
        return adjustedDistance;
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

    Vector2 currentInput;
    Vector2 _normalizedInput;
    protected float _acceleration = 0f;
    protected Vector2 _lerpedInput = Vector2.zero;
    public float Deceleration = 10f;
    public float Acceleration1 = 10f;
    protected Vector3 _movementVector;
    public float IdleThreshold = 0.05f;
    protected float _movementSpeed;
    private float _movementSpeedMultiplier = 1;

    public float ContextSpeedMultiplier = 1;

    public virtual float MovementSpeedMaxMultiplier { get; set; } = float.MaxValue;

    public float MovementSpeedMultiplier
    {
        get => Mathf.Min(_movementSpeedMultiplier, MovementSpeedMaxMultiplier);
        set => _movementSpeedMultiplier = value;
    }

    void SetMovement()
    {
        currentInput.x = Input.GetAxis("Horizontal");
        currentInput.y = Input.GetAxis("Vertical");

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

        _movementVector.x = _lerpedInput.x;
        _movementVector.y = 0f;
        _movementVector.z = _lerpedInput.y;
        _movementVector *= 6;

        if ((currentInput.magnitude <= IdleThreshold) && (CurrentMovement.magnitude < IdleThreshold))
        {
            print("4");
            _movementVector = Vector3.zero;
        }

        SetMove(_movementVector);
    }

    public Vector3 CurrentMovement;

    void SetMove(Vector3 movement)
    {
        CurrentMovement = movement;
        Vector3 directionVector;
        directionVector = movement;
        if (directionVector != Vector3.zero)
        {
            float directionLength = directionVector.magnitude;
            directionVector = directionVector / directionLength;
            directionLength = Mathf.Min(1, directionLength);
            directionLength = directionLength * directionLength;
            directionVector = directionVector * directionLength;
        }

        InputMoveDirection = transform.rotation * directionVector;
    }

    private void ProcessUpdate()
    {
        // if (_transform == null)
        // {
        //     return;
        // }

        _newVelocity = Velocity;
        _positionLastFrame = _transform.position;
        AddInput();
        AddGravity();
        MoveWithPlatform();
        ComputeVelocityDelta();
        MoveCharacterController();
        DetectNewMovingPlatform();
        ComputeNewVelocity();
        HandleAdvancedGroundDetection();
        HandleGroundContact();
        ComputeSpeed();
        DetermineDirection();
        RotateToFaceMovementDirection();
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
        _newVelocity += AddedForce;
        AddedForce = Vector3.zero;
    }

    protected virtual void GetMovingPlatformVelocity()
    {
        if (_movingPlatformCurrentHitCollider != null)
        {
            if (!_newMovingPlatform && (Time.deltaTime != 0f))
            {
                _movingPlatformVelocity = (
                    _movingPlatformCurrentHitCollider.localToWorldMatrix.MultiplyPoint3x4(
                        _movingPlatformCurrentHitColliderLocal)
                    - _lastMovingPlatformMatrix.MultiplyPoint3x4(_movingPlatformCurrentHitColliderLocal)
                ) / Time.deltaTime;
            }

            _lastMovingPlatformMatrix = _movingPlatformCurrentHitCollider.localToWorldMatrix;
            _newMovingPlatform = false;
        }
        else
        {
            _movingPlatformVelocity = Vector3.zero;
        }
    }

    void MoveWithPlatform()
    {
        if (_movingPlatformCurrentHitCollider == null) return;
        _moveWithPlatformMoveDistance.x = _moveWithPlatformMoveDistance.y = _moveWithPlatformMoveDistance.z = 0f;
        _moveWithPlatformGlobalPoint =
            _movingPlatformCurrentHitCollider.TransformPoint(_movingPlatformCurrentHitColliderLocal);
        _moveWithPlatformMoveDistance = (_moveWithPlatformGlobalPoint - _movingPlatformCurrentGlobalPoint);
        if (_moveWithPlatformMoveDistance != Vector3.zero)
        {
            _characterController.Move(_moveWithPlatformMoveDistance);
        }

        _moveWithPlatformGlobalRotation = _movingPlatformCurrentHitCollider.rotation * _movingPlatformLocalRotation;
        _moveWithPlatformRotationDiff =
            _moveWithPlatformGlobalRotation * Quaternion.Inverse(_movingPlatformGlobalRotation);
        float yRotation = _moveWithPlatformRotationDiff.eulerAngles.y;
        if (yRotation != 0)
        {
            _transform.Rotate(0, yRotation, 0);
        }
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

        _collisionFlags = _characterController.Move(_motion); // controller move

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
    public Vector3 ModelDirection;
    public Vector3 ModelAngles;

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

        ModelDirection = MovementRotatingModel.transform.forward.normalized;
        ModelAngles = MovementRotatingModel.transform.eulerAngles;
        MovementRotatingModel.transform.rotation = _newMovementQuaternion;
    }

    //k can
    void DetectNewMovingPlatform()
    {
        if (_movingPlatformCurrentHitCollider != _movingPlatformHitCollider)
        {
            if (_movingPlatformHitCollider != null)
            {
                _movingPlatformCurrentHitCollider = _movingPlatformHitCollider;
                _lastMovingPlatformMatrix = _movingPlatformHitCollider.localToWorldMatrix;
                _newMovingPlatform = true;
            }
        }
    }

    void ComputeNewVelocity()
    {
        Velocity = _newVelocity;
        Acceleration = (Velocity - VelocityLastFrame) / Time.deltaTime;
    }

    void HandleGroundContact()
    {
        if (_movingPlatformCurrentHitCollider == null) return;

        _movingPlatformCurrentHitColliderLocal =
            _movingPlatformCurrentHitCollider.InverseTransformPoint(_movingPlatformCurrentGlobalPoint);
        _movingPlatformGlobalRotation = _transform.rotation;
        _movingPlatformLocalRotation = Quaternion.Inverse(_movingPlatformCurrentHitCollider.rotation) *
                                       _movingPlatformGlobalRotation;
    }

    void ComputeSpeed()
    {
        if (Time.deltaTime != 0f)
        {
            Speed = (this.transform.position - _positionLastFrame) / Time.deltaTime;
        }

        // we round the speed to 2 decimals
        Speed.x = Mathf.Round(Speed.x * 100f) / 100f;
        Speed.y = Mathf.Round(Speed.y * 100f) / 100f;
        Speed.z = Mathf.Round(Speed.z * 100f) / 100f;
        _positionLastFrame = this.transform.position;
    }
}