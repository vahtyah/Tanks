using UnityEngine;

public interface ICharacterController
{
    public bool isButtonPauseDown { get; }
    Vector3 GetDirection();
    bool GetFire();
    Vector3 GetAimDirection();
    void Move(Vector3 direction);
}