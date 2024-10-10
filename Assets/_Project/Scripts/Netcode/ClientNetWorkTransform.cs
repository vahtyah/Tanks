using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetWorkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
