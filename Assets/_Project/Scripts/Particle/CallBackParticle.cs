using System;
using Unity.Netcode;
using UnityEngine;

public class CallBackParticle : NetworkBehaviour
{
    private void OnParticleSystemStopped()
    {
        if (NetworkObject != null)
        {
            if (!IsServer)
                return;
            NetworkObject.Despawn(true);
        }
        else
        {
            Pool.Return(gameObject);
        }
    }
}