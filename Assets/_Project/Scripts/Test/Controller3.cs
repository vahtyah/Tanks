using System;
using UnityEngine;

public class Controller3 : MonoBehaviour
{
    public float Speed = 6;
    public float Acceleration = 10;
    public float Deceleration = 10;
    
    private Vector2 currentInput;
    private Vector2 _normalizedInput;
    private Vector2 _lerpedInput;
    
    private float _acceleration;
    
    private Vector3 _movementVector;
    
    private Vector3 _newVelocity;
    
    private Vector3 CurrentMovement;
    
    private float _movementSpeedMultiplier = 1;
    
    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private float MovementSpeedMaxMultiplier { get; set; } = float.MaxValue;
    
    private float MovementSpeedMultiplier
    {
        get => Mathf.Min(_movementSpeedMultiplier, MovementSpeedMaxMultiplier);
        set => _movementSpeedMultiplier = value;
    }
    
    private void ProcessUpdate()
    {
        SetMovement();
        // ComputeVelocityDelta();
    }
    
    private void FixedUpdate()
    {
        ProcessUpdate();
    }
    
    
    private void SetMovement()
    {
        currentInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _lerpedInput = Vector2.Lerp(_lerpedInput, _normalizedInput, Time.deltaTime * 10);
        _normalizedInput = currentInput.normalized;
        if (_normalizedInput.magnitude == 0 || Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical"))
        {
            _acceleration = Mathf.Lerp(_acceleration, 0f, Deceleration * Time.deltaTime);
            _lerpedInput = Vector2.Lerp(_lerpedInput, _lerpedInput * _acceleration, Time.deltaTime * Deceleration);
        }
        else
        {
            _acceleration = Mathf.Lerp(_acceleration, 1f, Acceleration * Time.deltaTime);
            _lerpedInput = Vector2.ClampMagnitude(_normalizedInput, _acceleration);
        }
        
        _movementVector = new Vector3(_lerpedInput.x, 0f, _lerpedInput.y) * Speed;
        _characterController.Move(_movementVector * Time.deltaTime);
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
        {
            return;
        }
        
        body.AddForce(hit.moveDirection * 10);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Speed: {Speed}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Acceleration: {Acceleration}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Deceleration: {Deceleration}");
    }

}