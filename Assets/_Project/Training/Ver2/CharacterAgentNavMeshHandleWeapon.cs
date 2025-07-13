using UnityEngine;

public class CharacterAgentNavMeshHandleWeapon : MonoBehaviour
{
    [SerializeField] private WeaponAgentNavMesh weapon;
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
        weapon.SetOwner(GetComponent<AgentNavMesh>());
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