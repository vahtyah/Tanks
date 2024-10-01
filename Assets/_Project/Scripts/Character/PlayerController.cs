using System;
using UnityEngine;

public class PlayerController : MonoBehaviour, ICharacterController
{
    private PlayerCharacter character;
    private Rigidbody rb;
    private string playerID;
    private Camera cam;
    private bool isButtonPauseDown1;
    private string axisHorizontal { get; set; }
    private string axisVertical { get; set; }
    private string fireButton { get; set; }
    private string pauseButton { get; set; }

    public Vector3 Direction { get; private set; }

    private void Awake()
    {
        cam = Camera.main; 
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        character = GetComponent<PlayerCharacter>();
        playerID = character.PlayerID;
        InitializeAxis();
    }

    private void InitializeAxis()
    {
        playerID ??= "Player1";
        axisHorizontal = playerID + "_Horizontal";
        axisVertical = playerID + "_Vertical";
        fireButton = playerID + "_Fire";
        pauseButton = playerID + "_Pause";
    }
    
    private void Update()
    {
        isButtonPauseDown1 = GetPauseButton();
    }

    public bool GetPauseButton() { return Input.GetButtonDown(pauseButton); }

    bool ICharacterController.isButtonPauseDown => GetPauseButton();

    public Vector3 GetDirection() { return new Vector3(Input.GetAxis(axisHorizontal), 0, Input.GetAxis(axisVertical)); }

    public bool GetFire() { return Input.GetButtonDown(fireButton); }

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
}