using System;
using Photon.Pun;
using Testing;
using UnityEngine;

public class NetworkPlayerMovement : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 5f;
    public bool HasShootingInput { get; private set; }
    public event Action OnRPCReceived;
    
    void Update()
    {
        if (photonView.IsMine)
        {
            float horizontalInput = InputSimulator.GetAxisValue("Horizontal");
            float verticalInput = InputSimulator.GetAxisValue("Vertical");
            
            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;
            transform.Translate(movement);
            
            if (InputSimulator.GetAxisValue("Fire1") > 0)
            {
                HasShootingInput = true;
                photonView.RPC("FireWeapon", RpcTarget.All);
            }
            else
            {
                HasShootingInput = false;
            }
        }
    }
    
    [PunRPC]
    void FireWeapon()
    {
        // This would normally handle weapon firing
        OnRPCReceived?.Invoke();
    }
}