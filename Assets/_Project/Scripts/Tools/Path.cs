using System;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private List<Vector3> pathPoints = new();

    public Vector3 CurrentPoint { get; private set; }
    
    private ISpawnPoint spawner;
    
    private void Awake()
    {
        spawner = new LinearSpawnPoint(pathPoints);
        CurrentPoint = transform.position;
    }
    
    public Vector3 NextSpawnPoint()
    {
        CurrentPoint = spawner.NextSpawnPoint();
        return CurrentPoint;
    }
    
    public Vector3 ClosestSpawnPoint(Vector3 position)
    {
        return spawner.ClosestSpawnPoint(position);
    }
    
    private void OnDrawGizmos()
    {
        for (int i = 0; i < pathPoints.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pathPoints[i], 0.5f);
            if (i < pathPoints.Count - 1)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
            }
        }
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pathPoints[^1], pathPoints[0]);
    }
}
