using System;
using Unity.MLAgents;
using UnityEngine;

public class CharacterAgentHandleWeapon : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private CharacterModel characterModel;
    private Transform projectileSpawnPoint;

    private void Awake()
    {
        projectileSpawnPoint = characterModel.PrimaryProjectileSpawnPoint;
        EquipWeapon();
    }

    private void EquipWeapon()
    {
        weapon.SetProjectileSpawnPoint(projectileSpawnPoint);
        weapon.SetOwner(GetComponent<CharacterAgent>());
    }
    
    public void UseWeapon()
    {
        weapon.UseWeapon();
    }
    
    public bool IsReadyToFire()
    {
        return weapon.weaponCooldownTimer.IsCompleted;
    }
}