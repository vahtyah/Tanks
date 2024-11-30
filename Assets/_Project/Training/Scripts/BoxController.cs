using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoxController : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        transform.localPosition = new Vector3(Random.Range(-20, 20), transform.localPosition.y, Random.Range(-20, 20));
    }
}
