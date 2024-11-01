using UnityEngine;

public interface ICharacterController
{
    Vector3 GetDirection();
    bool GetFire();
    Vector3 GetAimDirection();
    void Move(Vector3 direction);
    void Reset();
    Rigidbody GetRigidbody();
    void AddOnTriggerEnter(System.Action<Collider> action);
}