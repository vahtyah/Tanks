using System;
using Photon.Pun;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("OnTriggerEnter");
    }
}