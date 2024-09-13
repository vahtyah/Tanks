using System;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    private Character character;
    private string playerID;
    private string axisHorizontal { get; set; }
    private string axisVertical { get; set; }
    private string fireButton { get; set; }

    private void Start()
    {
        character = GetComponent<Character>();
        playerID = character.PlayerID;
        InitializeAxis();
    }

    private void InitializeAxis()
    {
        playerID ??= "Player1";      
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
    
    public bool GetFireButton()
    {
        return Input.GetButton(fireButton);
    }
}
