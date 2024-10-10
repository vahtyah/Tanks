using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class DamageOnTouch : NetworkBehaviour
{
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float damage = 1;
    [SerializeField] private float damageTaken;

    [SerializeField] private List<GameObject> ignoreObjects = new();
    [SerializeField] private MMFeedbacks onHitFeedback;
    [SerializeField] private ParticleSystem projectileHitParticles;
    [SerializeField] private Projectile projectile;

    private Health damageTakenHealth;

    private void Initialization()
    {
        damageTakenHealth = GetComponent<Health>();
        Pool.Register(projectileHitParticles.gameObject);
        projectile = GetComponent<Projectile>();
        //clear
    }

    private void Start() { Initialization(); }

    public void AddIgnoreObject(GameObject gameObject)
    {
        var networkObject = gameObject.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            AddIgnoreObjectServerRpc(networkObject.NetworkObjectId); // Gửi NetworkObjectId lên server
        }
        else
        {
            ignoreObjects.Add(gameObject);
        }
    }

    [ServerRpc]
    private void AddIgnoreObjectServerRpc(ulong networkObjectId)
    {
        SendLogToAllClientsClientRpc(networkObjectId); // Gửi thông báo cho tất cả client
    }

    [ClientRpc]
    private void SendLogToAllClientsClientRpc(ulong networkObjectId)
    {
        NetworkObject targetObject =
            NetworkManager.Singleton.SpawnManager
                .SpawnedObjects[networkObjectId]; // Lấy lại đối tượng từ NetworkObjectId
        ignoreObjects.Add(targetObject.gameObject);
    }

    public void RemoveIgnoreObject(GameObject gameObject) { ignoreObjects.Remove(gameObject); }

    public void ClearIgnoreObjects() { ignoreObjects.Clear(); }

    private void OnTriggerEnter(Collider other)
    {
        if (!EvaluateAvailability(other.gameObject)) return;
        if (NetworkManager != null)
        {
            if (!IsOwner) return;
            SpawnHitParticlesServerRpc();
            PlayParticleServerRpc();
            var health = other.gameObject.GetComponent<HealthNetwork>();
            if (health != null)
            {
                if (health.TryKill(damage))
                {
                    var playerLeft = LevelManagerNetCode.Instance.playerCharacters;
                    if (playerLeft.Count <= 1)
                    {
                        NetworkManager.Singleton.ConnectedClients[playerLeft.First().Key].PlayerObject
                            .GetComponent<PlayerCharacter>().NotifyWinClientRpc();
                    }
                }
            }
        }
        else
        {
            onHitFeedback?.PlayFeedbacks();
            Pool.Spawn(projectileHitParticles.gameObject, transform.position, quaternion.identity);
            var health = other.gameObject.GetComponent<Health>();
            if (health != null) health.TakeDamage(damage);
        }

        SelfDamage();
    }

    [ServerRpc]
    private void SpawnHitParticlesServerRpc()
    {
        NetworkObjectSpawner.Spawn(projectileHitParticles.gameObject, transform.position,
            quaternion.identity);
    }

    [ServerRpc] //yêu cầu server kích hoạt particle system
    private void PlayParticleServerRpc() { PlayParticleClientRpc(); }

    [ClientRpc] //ClientRpc để kích hoạt particle trên tất cả các client
    private void PlayParticleClientRpc() { onHitFeedback?.PlayFeedbacks(); }

    private void SelfDamage()
    {
        if (damageTakenHealth == null) return;
        damageTakenHealth.TakeDamage(damageTaken);
    }

    private bool EvaluateAvailability(GameObject otherGameObject)
    {
        return !ignoreObjects.Contains(otherGameObject) && IsLayerValid(otherGameObject);
    }

    private bool IsLayerValid(GameObject otherGameObject)
    {
        return (targetLayerMask & 1 << otherGameObject.layer) != 0;
    }
}