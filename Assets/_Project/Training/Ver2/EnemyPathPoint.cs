using System.Collections.Generic;
using UnityEngine;

public class EnemyPathPoint : SerializedSingleton<EnemyPathPoint>
{
    public Transform[] PathPoints;

     public Transform GetClosestTransform(Transform currentTeTransform, Transform previousTranform = null)
     {
        Transform closestTransform = null;
        float closestDistance = Mathf.Infinity;

        foreach (var pathPoint in PathPoints)
        {
            if(pathPoint == currentTeTransform || pathPoint == previousTranform)
                continue; 
            float distance = Vector3.Distance(currentTeTransform.position, pathPoint.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTransform = pathPoint;
            }
        }

        return closestTransform;
     }
     
     public Transform GetTransformInFrontWithinAngle(Transform currentTransform, Transform target, float maxAngle = 45f)
     {
         Transform selectedTransform = null;
         float closestDistance = Mathf.Infinity;
    
         foreach (var pathPoint in PathPoints)
         {
             if (pathPoint == currentTransform || pathPoint == target)
                 continue;
            
             Vector3 directionToPoint = pathPoint.position - currentTransform.position;
             float angle = Vector3.Angle(currentTransform.forward, directionToPoint);
        
             // Check if point is within the specified angle limit
             if (angle <= maxAngle)
             {
                 float distance = Vector3.Distance(currentTransform.position, pathPoint.position);
                 if (distance < closestDistance)
                 {
                     closestDistance = distance;
                     selectedTransform = pathPoint;
                 }
             }
         }
    
         return selectedTransform;
     }
     
     public Transform GetTransformWithAngleGreaterThan90(Transform currentTransform, Vector3 forwardDirection)
     {
         Transform selectedTransform = null;
    
         foreach (var pathPoint in PathPoints)
         {
             if (pathPoint == currentTransform)
                 continue;
            
             Vector3 directionToPoint = (pathPoint.position - currentTransform.position).normalized;
             float angle = Vector3.Angle(forwardDirection, directionToPoint);
        
             // Check if angle is greater than 90 degrees
             if (angle < 180f)
             {
                 selectedTransform = pathPoint;
                 break; // Return the first point that satisfies the condition
            
                 // Alternatively, you could continue looking and choose based on other criteria:
                 // - The point with the largest angle
                 // - The closest point that satisfies the angle condition
             }
         }
         
         if(selectedTransform == null)
         {
             // If no point is found, return the closest one
             selectedTransform = GetClosestTransform(currentTransform);
         }
    
         return selectedTransform;
     }
     
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var pathPoint in PathPoints)
        {
            Gizmos.DrawSphere(pathPoint.position, 0.5f);
        }
    }

    public Transform GetRandomPathPoint()
    {
        if (PathPoints.Length == 0)
        {
            Debug.LogWarning("No path points available.");
            return null;
        }

        int randomIndex = Random.Range(0, PathPoints.Length);
        return PathPoints[randomIndex];
    }
}