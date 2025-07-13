using Unity.MLAgents;
using UnityEngine;

public class DamageOnTouchAgent : DamageOnTouch
{
    ProjectileAgent projectileAgent;

    protected override void Awake()
    {
        base.Awake();
        projectileAgent = GetComponent<ProjectileAgent>();
    }

    public override void OnTriggerEnter(Collider collider)
    {
        var targetGameObject = collider.gameObject;
        if(!IsTargetAvailable(targetGameObject)) return;
        if(IsSameTeamAgent(targetGameObject)) return;
        
        // PreventPassThrough();
        if(collider.TryGetComponent(out CharacterAgent agent))
        {
            projectileAgent.agent.AddReward(.1f);
            agent.AgentDie();
        }
        else
        {
            projectileAgent.agent.AddReward(-.00001f);
        }
        
        Pool.Despawn(gameObject);
    }

    private bool IsSameTeamAgent(GameObject targetGameObject)
    {
        if (targetGameObject.TryGetComponent(out CharacterAgent agent))
        {
            return projectileAgent.agent.GetTeamId() == agent.GetTeamId();
        }

        return false;
    }
}
