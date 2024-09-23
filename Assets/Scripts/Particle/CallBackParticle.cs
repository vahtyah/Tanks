using System;
using UnityEngine;

public class CallBackParticle : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        Pool.Return(gameObject);
    }
}
