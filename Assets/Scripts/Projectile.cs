using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    private Rigidbody rb;
    Vector3 moveDirection;
    public float Acceleration = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    private void Update()
    {
        Movement();
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    void Movement()
    {
        moveDirection = transform.forward * ((speed / 10) * Time.deltaTime);
        rb.MovePosition(rb.position + moveDirection);
        speed += Acceleration * Time.deltaTime;
    }
}
