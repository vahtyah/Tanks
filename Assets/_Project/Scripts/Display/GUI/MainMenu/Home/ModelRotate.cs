using System;
using UnityEngine;

public class ModelRotate : MonoBehaviour
{
    
    [SerializeField] private float rotateSpeed = 10;
    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }
}
