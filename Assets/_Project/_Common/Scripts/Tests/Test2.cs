using System;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            Pool.Spawn(gameObject, null);
        
        if(Input.GetKeyDown(KeyCode.O))
            Pool.Despawn(gameObject);
    }
}