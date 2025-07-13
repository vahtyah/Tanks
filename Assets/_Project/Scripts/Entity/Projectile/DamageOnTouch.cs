using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

public interface ITrigger
{
    void OnTriggerEnter(Collider collider);
}

public class DamageOnTouch : MonoBehaviour, ITrigger
{
    [SerializeField, BoxGroup("Target Settings", showLabel:false)] private LayerMask targetLayerMask;
    [SerializeField, BoxGroup("Damage Settings")]
    private float damage = 1;

    [SerializeField, BoxGroup("Damage Settings")]
    private float damageTaken;

    [SerializeField, BoxGroup("Feedbacks")]
    protected MMFeedbacks deathFeedbacks;

    [Log] private List<GameObject> ignoredObjects = new();
    private Character owner;
    private HealthTest health;
    private Vector3 initialPosition;

    protected virtual void Awake()
    {
        health = GetComponent<HealthTest>();
    }

    private void OnEnable()
    {
        initialPosition = transform.position;
    }


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

    public virtual void OnTriggerEnter(Collider collider)
    {
        GameObject targetObject = collider.gameObject;
        if (!IsTargetAvailable(targetObject) || IsSameTeam(targetObject)) return;

        PreventPassThrough();
        if (targetObject.TryGetComponent(out ChaHealth targetHealth))
        {
            if (targetHealth.IsInvulnerable)
            {
                ApplySelfDamage();
                return;
            }

            targetHealth.TakeDamage(damage, owner);
        }

        GenerateHitParticles(transform.position);
        ApplySelfDamage();
    }

    private readonly RaycastHit[] raycastHitsCache = new RaycastHit[1]; // Cache array dùng cho RaycastNonAlloc

    protected void PreventPassThrough()
    {
        var movementDirection = transform.position - initialPosition;

        if (movementDirection.sqrMagnitude > 0.1f)
        {
            var normalizedDirection = movementDirection.normalized;
            var distanceMoved = movementDirection.magnitude;

            if (Physics.RaycastNonAlloc(initialPosition, normalizedDirection, raycastHitsCache, distanceMoved,
                    targetLayerMask) > 0)
            {
                transform.position = raycastHitsCache[0].point - normalizedDirection * 0.1f;
            }
        }
    }


    private void GenerateHitParticles(Vector3 position)
    {
        deathFeedbacks?.PlayFeedbacks(position);
    }

    public void SetOwner(Character character)
    {
        owner = character;
    }

    private void ApplySelfDamage()
    {
        if (health != null)
        {
            health.TakeDamage(damageTaken, owner);
        }
        else
        {
            Pool.Despawn(gameObject);
        }
    }

    protected bool IsTargetAvailable(GameObject targetObject)
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