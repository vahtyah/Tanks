using System;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour, ICharacterController
{
    private PlayerCharacter character;
    private Rigidbody rb;
    private Camera cam;
    private InputManager input;
    private bool isButtonPauseDown1;
    public Vector3 Direction { get; private set; }

    private Vector3 networkPosition;
    private Quaternion networkRotation;
    public Collider Collider { get; private set; }
    private Action<Collider> onTriggerEnter;

    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        character = GetComponent<PlayerCharacter>();
        input = new InputManager(character.PlayerID);
        Collider = GetComponent<Collider>();
    }

    private void Update() { isButtonPauseDown1 = GetPauseButton(); }

    public bool GetPauseButton() { return input.GetPauseButton(); }

    public Vector3 GetDirection() { return input.GetDirection(); }

    public bool GetFire() { return input.GetFire(); }

    public Vector3 GetAimDirection()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = cam.WorldToScreenPoint(transform.position).z;
        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(mouseScreenPosition);
        Vector3 direction = (mouseWorldPosition - transform.position).With(y: 0);
        return direction;
    }

    public void Move(Vector3 direction)
    {
        Direction = direction;
        rb.AddForce(direction, ForceMode.VelocityChange);
    }

    public void Reset() { }

    public Rigidbody GetRigidbody() { return rb; }

    public void AddOnTriggerEnter(Action<Collider> action) { onTriggerEnter += action; }
}