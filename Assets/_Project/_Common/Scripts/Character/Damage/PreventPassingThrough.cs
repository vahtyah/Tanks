using System;
using UnityEngine;

public class PreventPassingThrough : MonoBehaviour
{
    public LayerMask ObstaclesLayerMask; 
    private Collider collider;
    private float sqrBoundsWidth;
    private static readonly RaycastHit[] raycastHits = new RaycastHit[1]; // Cache array dùng cho RaycastNonAlloc

    
    private Vector3 positionLastFrame;
    private ITrigger trigger;
    
    private void Start(){

        collider = GetComponent<Collider>();
        trigger = GetComponent<ITrigger>();
        
        if(trigger == null)
            throw new Exception("PreventPassingThrough requires a component that implements ITrigger");
        
        var adjustmentDistance = Mathf.Min(Mathf.Min(collider.bounds.extents.x, collider.bounds.extents.y),
            collider.bounds.extents.z);
        var adjustedDistance = adjustmentDistance * (1.0f - 0.1f);
        sqrBoundsWidth = adjustedDistance * adjustedDistance;
        
        collider.enabled = false;
    }

    private void OnEnable()
    {
        positionLastFrame = transform.position;
    }
    
    private void OnDisable()
    {
        positionLastFrame = Vector3.zero;
    }

    private void FixedUpdate()
    {
        var movementThisFrame = transform.position - positionLastFrame;
        var movementThisFrameSqrMagnitude = movementThisFrame.sqrMagnitude;
        
        if (movementThisFrameSqrMagnitude > sqrBoundsWidth)
        {
            var direction = movementThisFrame.normalized;
            var distance = Mathf.Sqrt(movementThisFrameSqrMagnitude);
            if (Physics.RaycastNonAlloc(positionLastFrame, direction, raycastHits, distance, ObstaclesLayerMask) > 0)
            {
                transform.position = raycastHits[0].point - direction * 0.1f;
                trigger.OnTriggerEnter(raycastHits[0].collider);
            }
        }
        
        positionLastFrame = transform.position;
    }
}
