using UnityEngine;

public class DamageOnTouchNavMesh : DamageOnTouch
{
    ProjectileNavMesh projectileAgent;
    public ParticleSystem explosionEffect;

    protected override void Awake()
    {
        base.Awake();
        projectileAgent = GetComponent<ProjectileNavMesh>();
    }

    public override void OnTriggerEnter(Collider collider)
    {
        var targetGameObject = collider.gameObject;
        if(!IsTargetAvailable(targetGameObject)) return;
        Debug.Log("1");
        if(IsSameTeamAgent(targetGameObject)) return;
        Debug.Log("2");
        PreventPassThrough();
        if(collider.TryGetComponent(out AgentNavMesh agent))
        {
            Debug.Log("Hit agent");
            agent.Health -= 10;
        }
        
        deathFeedbacks?.PlayFeedbacks(transform.position);
        if (explosionEffect != null)
        {
            Pool.Spawn(explosionEffect.gameObject, transform.position, explosionEffect.transform.rotation);
        }
        Pool.Despawn(gameObject);
    }
    
    public bool IsSameTeamAgent(GameObject targetGameObject)
    {
        if (targetGameObject.TryGetComponent(out AgentNavMesh agent))
        {
            return projectileAgent.agent.GetTeamId() == agent.GetTeamId();
        }

        return false;
    }

}