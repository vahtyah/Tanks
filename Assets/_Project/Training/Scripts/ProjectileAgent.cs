using Unity.MLAgents;
using UnityEngine;

public class ProjectileAgent : Projectile
{
    DamageOnTouchAgent damageOnTouchAgent;
    public CharacterAgent agent { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
        damageOnTouchAgent = GetComponent<DamageOnTouchAgent>();
    }

    public override Projectile SetOwner(CharacterAgent agent)
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
