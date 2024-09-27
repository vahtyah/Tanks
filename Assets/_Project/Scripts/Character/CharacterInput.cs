using System;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    private Character character;
    private string playerID;
    private Camera cam;
    private string axisHorizontal { get; set; }
    private string axisVertical { get; set; }
    private string fireButton { get; set; }
    private string pauseButton { get; set; }
    public bool isButtonPauseDown { get; private set; }

    private void Awake() { cam = Camera.main; }

    private void Start()
    {
        character = GetComponent<Character>();
        playerID = character.PlayerID;
        InitializeAxis();
    }

    private void Update() { isButtonPauseDown = GetPauseButton(); }

    private void InitializeAxis()
    {
        playerID ??= "Player1";
        axisHorizontal = playerID + "_Horizontal";
        axisVertical = playerID + "_Vertical";
        fireButton = playerID + "_Fire";
        pauseButton = playerID + "_Pause";
    }

    public float GetAxisHorizontal() { return Input.GetAxis(axisHorizontal); }

    public float GetAxisVertical() { return Input.GetAxis(axisVertical); }

    public bool GetFireButton() { return Input.GetButtonDown(fireButton); }

    public bool GetPauseButton() { return Input.GetButtonDown(pauseButton); }

    public Vector3 GetMouseWorldPosition(Vector3 position)
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = cam.WorldToScreenPoint(position).z;
        return cam.ScreenToWorldPoint(mouseScreenPosition);
    }
}