using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour, ICharacterController
{
    private PlayerCharacter character;
    private Rigidbody rb;
    private Camera cam;
    private InputManager input;
    private bool isButtonPauseDown1;
    public Vector3 Direction { get; private set; }

    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        character = GetComponent<PlayerCharacter>();
        input = new InputManager(character.PlayerID);
    }

    private void Update() { isButtonPauseDown1 = GetPauseButton(); }

    public bool GetPauseButton() { return input.GetPauseButton(); }

    bool ICharacterController.isButtonPauseDown => GetPauseButton();

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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
        CinemachineCameraControllerNetCode.Instance.SetFollowTarget(character.CameraTarget.transform);
        GUIManagerNetcode.Instance.SetHealthBar(character.HealthNetwork);
        character.transform.position = LevelManagerNetCode.Instance.spawner.NextRandomSpawnPoint();
    }
}