using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float moveSpeed = 10f; // Tốc độ di chuyển
    private Vector3 turnSpeed; // Tốc độ xoay
    private Rigidbody rb;
    public GameObject model;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleMovement();
    }
    
    protected Quaternion _tmpRotation;
    protected Quaternion _newMovementQuaternion;
    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        print(moveX + " " + moveY);
        Vector3 movement = new Vector3(moveX, 0f, moveY);;
        if (movement.magnitude > 0)
        {
            // Nếu có di chuyển, cập nhật turnSpeed và đặt cờ isMoving là true
            turnSpeed = movement.normalized;
        }
        
        if(turnSpeed != Vector3.zero)
        {
            _tmpRotation = Quaternion.LookRotation(turnSpeed);
            _newMovementQuaternion = Quaternion.Slerp(model.transform.rotation, _tmpRotation,
                Time.deltaTime * 10);
        }
        
        movement = movement * (moveSpeed * Time.deltaTime);
        
        model.transform.rotation = _newMovementQuaternion;

        rb.MovePosition(rb.position + movement);
    }
}
