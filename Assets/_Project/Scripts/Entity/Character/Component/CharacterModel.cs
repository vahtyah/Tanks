using Sirenix.OdinInspector;
using UnityEngine;

public class CharacterModel : MonoBehaviour
{
    [Required, BoxGroup("Tank")] public GameObject Wheels;
    [Required, BoxGroup("Tank")] public GameObject Body;
    [Required, BoxGroup("Tank")] public GameObject Turret;

    [Required, BoxGroup("Camera")] public Transform CameraTarget;
    [Required, BoxGroup("Flag")] public ForceFieldController FlagDisplay;
    [Required, BoxGroup("Flag")] public Renderer FlagRenderer;

    [Required, BoxGroup("Weapon")] public Transform PrimaryWeaponHolder;
    [Required, BoxGroup("Weapon")] public Transform PrimaryProjectileSpawnPoint;

    [Required, BoxGroup("Weapon"), Space] public Transform SkillWeaponHolder;
    [Required, BoxGroup("Weapon")] public Transform SkillSpawnPoint;
}