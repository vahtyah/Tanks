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
}
