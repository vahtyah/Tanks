using System;
using UnityEngine;

public class TankController : MonoBehaviour
{
    Rigidbody rb;
    public Vector3 CurrentDirection { get; private set; }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        MoveCharacter();
    }
    
    public void SetDirection(Vector3 direction)
    {
        this.CurrentDirection = direction;
    }

    private void MoveCharacter()
    {
        rb.AddForce(CurrentDirection, ForceMode.VelocityChange);
    }
    
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Velocity: {rb.velocity}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Acceleration: {rb.velocity.magnitude}");
        // GUI.Label(new Rect(10, 50, 300, 20), $"Deceleration: {}");
    }
}
