using UnityEngine;

public class ProjectileNavMesh : Projectile
{
    DamageOnTouchNavMesh damageOnTouchAgent;
    public AgentNavMesh agent;
    
    protected override void Awake()
    {
        base.Awake();
        damageOnTouchAgent = GetComponent<DamageOnTouchNavMesh>();
    }
    
    public ProjectileNavMesh SetOwner1(AgentNavMesh agent)
    {
        this.agent = agent;
        damageOnTouchAgent.AddToIgnoreList(agent.gameObject);
        return this;
    }

    public override void OnSpawn()
    {
        timer?.ReStart();
        damageOnTouchAgent.AddToIgnoreList(gameObject);
        currentSpeed = speed;
    }
}
