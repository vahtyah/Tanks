using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move2D : MonoBehaviour
{
    public float moveSpeed = 10f; // Tốc độ di chuyển
    public float turnSpeed = 100f; // Tốc độ xoay
    private Rigidbody rb;

    void Start() { rb = GetComponent<Rigidbody>(); }

    void Update() { HandleMovement(); }

    void HandleMovement()
    {
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float turn = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;

        // Tạo vector di chuyển cho xe tăng
        Vector3 movement = transform.forward * move;

        // Di chuyển và xoay xe tăng
        rb.MovePosition(rb.position + movement);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, turn, 0));
    }
}