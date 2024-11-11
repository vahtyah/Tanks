using UnityEngine;

public class InputManager
{
    private string playerID = "Player1";
    private string axisHorizontal { get; set; }
    private string axisVertical { get; set; }
    private string fireButton { get; set; }
    private string pauseButton { get; set; }
    
    public InputManager(string playerID)
    {
        this.playerID = playerID;
        InitializeAxis();
    }
    
    private void InitializeAxis()
    {
        axisHorizontal = $"{playerID}_Horizontal";
        axisVertical = $"{playerID}_Vertical";
        fireButton = $"{playerID}_Fire";
        pauseButton = $"{playerID}_Pause";
    }
    
    public bool GetPauseButton() { return Input.GetButtonDown(pauseButton); }
    public Vector3 GetDirection() { return new Vector3(Input.GetAxis(axisHorizontal), 0, Input.GetAxis(axisVertical)); }
    public bool GetFire() { return Input.GetButtonDown(fireButton); }
    public bool GetInputSkill()
    {
      return Input.GetMouseButton(1);   
    }
}