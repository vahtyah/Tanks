using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Photon.Pun;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float damage = 1;
    [SerializeField] private float damageTaken;
    
    [SerializeField] private List<GameObject> ignoreObjects = new();
    [SerializeField] private MMFeedbacks onHitFeedback;
    [SerializeField] private ParticleSystem projectileHitParticles;
    
    [SerializeField] private Character owner;
    
    private void Initialization()
    {
    }
    
    private void Start()
    {
        Initialization();
    }
    
    public void AddIgnoreObject(GameObject gameObject)
    {
        ignoreObjects.Add(gameObject);
    }
    
    public void RemoveIgnoreObject(GameObject gameObject)
    {
        ignoreObjects.Remove(gameObject);
    }
    
    public void ClearIgnoreObjects()
    {
        ignoreObjects.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!EvaluateAvailability(other.gameObject)) return;
        CreateHitParticles();
        var health = other.gameObject.GetComponent<CharacterHealth>();
        if (health != null)
            health.TakeDamage(damage, owner);
        SelfDamage();
    }
    
    private void CreateHitParticles()
    {
        var hitParticles = Pool.Spawn(projectileHitParticles.gameObject, transform.position).GetComponent<ParticleSystem>();
        hitParticles.Play();
        onHitFeedback?.PlayFeedbacks();
    }
    
    public void SetOwner(Character character)
    {
        owner = character;
    }

    private void SelfDamage()
    {
        Pool.Despawn(gameObject);
    }

    private bool EvaluateAvailability(GameObject otherGameObject)
    {
        return (!ignoreObjects.Contains(otherGameObject) && IsLayerValid(otherGameObject));
    }

    private bool IsLayerValid(GameObject otherGameObject)
    {
        return (targetLayerMask & 1 << otherGameObject.layer) != 0;        
    }
}
