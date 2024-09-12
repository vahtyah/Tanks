using System;
using UnityEngine;

public class TimerTester : MonoBehaviour
{
    private Timer timer;

    private void Start()
    {
        timer = Timer.Register(10f).OnUpdate(f => funcasd()).AutoDestroyWhenOwnerDestroyed(this).Start();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(gameObject);
        }
    }

    private void funcasd()
    {
        print("a");
    }
}
