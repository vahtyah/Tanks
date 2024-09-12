using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Character character;
    private string playerID;
    public string axisHorizontal { get; private set; }
    public string axisVertical { get; private set; }
    public string fireButton { get; private set; }

    private void Start()
    {
        character = GetComponent<Character>();
        playerID = character.PlayerID;
        InitializeAxis();
    }
    
    

    private void InitializeAxis()
    {
        axisHorizontal = playerID + "_Horizontal";
        axisVertical = playerID + "_Vertical";
        fireButton = playerID + "_Fire";
    }

    public float GetAxisHorizontal()
    {
        return Input.GetAxis(axisHorizontal);        
    }
    
    public float GetAxisVertical()
    {
        return Input.GetAxis(axisVertical);        
    }
}
