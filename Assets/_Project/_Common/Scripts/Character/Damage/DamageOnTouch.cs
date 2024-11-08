using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float damage = 1;
    [SerializeField] private float damageTaken;

    [ShowInInspector, TitleGroup("Debugs")]
    private List<GameObject> ignoredObjects = new();
    [SerializeField] private MMFeedbacks onHitFeedback;
    [SerializeField] private ParticleSystem hitParticles;
    private Character owner;

    private void Initialize()
    {
    }

    private void Start()
    {
        Initialize();
    }

    public void IgnoreTeamMembers()
    {
        // if (owner == null) return;
        // var ownerTeam = PhotonNetwork.LocalPlayer.GetTeam();
        // var teamMembers = Team.GetPlayersByTeam(ownerTeam.TeamType);
        // foreach (var teamMember in teamMembers)
        // {
        //     Debug.Log(teamMember.ActorNumber);
        //     //TODO: change player to Character in TeamManager
        //     AddIgnoreObject(RoomManager.Instance.PlayerCharacters[teamMember.ActorNumber].gameObject);
        // }
    }

    public void AddToIgnoreList(GameObject gameObject)
    {
        ignoredObjects.Add(gameObject);
    }

    public void RemoveFromIgnoreList(GameObject gameObject)
    {
        ignoredObjects.Remove(gameObject);
    }

    public void ClearIgnoreList()
    {
        ignoredObjects.Clear();
    }

    private void OnTriggerEnter(Collider collider)
    {
        GameObject targetObject = collider.gameObject;

        if (!IsTargetAvailable(targetObject) || IsSameTeam(targetObject)) return;

        if (targetObject.TryGetComponent(out CharacterHealth targetHealth))
        {
            if (targetHealth.IsInvulnerable)
            {
                ApplySelfDamage();
                return;
            }

            targetHealth.TakeDamage(damage, owner);
        }

        GenerateHitParticles();
        ApplySelfDamage();
    }


    private void GenerateHitParticles()
    {
        var hitParticles = Pool.Spawn(this.hitParticles.gameObject, transform.position)
            .GetComponent<ParticleSystem>();
        hitParticles.Play();
        onHitFeedback?.PlayFeedbacks();
    }

    public void SetOwner(Character character)
    {
        owner = character;
    }

    private void ApplySelfDamage()
    {
        Pool.Despawn(gameObject);
    }

    private bool IsTargetAvailable(GameObject targetObject)
    {
        return !ignoredObjects.Contains(targetObject) && IsLayerInTargetMask(targetObject.layer);
    }

    private bool IsSameTeam(GameObject targetObject)
    {
        if (!targetObject.TryGetComponent(out Character targetPlayer)) return false;
        if (owner.Teammates.Contains(targetPlayer)) return true;

        bool isSameTeam = targetPlayer.GetTeamType() == owner.GetTeamType();
        if (isSameTeam) owner.Teammates.Add(targetPlayer);

        return isSameTeam;
    }


    private bool IsLayerInTargetMask(int layer)
    {
        return (targetLayerMask & (1 << layer)) != 0;
    }
}