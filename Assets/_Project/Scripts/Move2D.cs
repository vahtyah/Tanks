using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move2D : MonoBehaviour
{
    [SerializeField] private string playerID = "Player1";

    public string axisHorizontal { get; private set; }
    public string axisVertical { get; private set; }
    public string fireButton { get; private set; }
    
    public void InitializeAxis()
    {
        axisHorizontal = playerID + "_Horizontal";
        axisVertical = playerID + "_Vertical";
        fireButton = playerID + "_Fire";
    }
    public void SetPlayerID(string id)
    {
        playerID = id;
        InitializeAxis();
    }
}