using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float damage = 1;
    [SerializeField] private float damageTaken;
    
    [SerializeField] private List<GameObject> ignoreObjects = new();
    private Health damageTakenHealth;

    private void Initialization()
    {
        damageTakenHealth = GetComponent<Health>();
        //clear
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
        var health = other.gameObject.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(damage);
        SelfDamage();
    }

    private void SelfDamage()
    {
        if (damageTakenHealth == null) return;
        damageTakenHealth.TakeDamage(damageTaken);        
    }

    private bool EvaluateAvailability(GameObject otherGameObject)
    {
        return !ignoreObjects.Contains(otherGameObject) && IsLayerValid(otherGameObject);
    }

    private bool IsLayerValid(GameObject otherGameObject)
    {
        return (targetLayerMask & 1 << otherGameObject.layer) != 0;        
    }
}
