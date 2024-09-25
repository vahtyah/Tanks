using UnityEngine;

public class CharacterHealth : Health
{
    public GameObject tankExplosionEffect;

    protected override void Start()
    {
        base.Start();
        Pool.Register(tankExplosionEffect);
    }

    protected override void OnDeath()
    {
        character.conditionState.ChangeState(CharacterStates.CharacterCondition.Dead);
        Event.Trigger(EventType.PlayerDeath, GetComponent<Character>());
        var effect = Pool.Get(tankExplosionEffect).GetComponent<ParticleSystem>();
        effect.transform.position = transform.position;
        effect.Play();
        gameObject.SetActive(false);
    }
}
