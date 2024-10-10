using System.Collections.Generic;
using UnityEngine;

public interface ISpawnPoint
{
    Vector3 NextSpawnPoint();
    Vector3 NextSpawnPoint(int index);
    Vector3 ClosestSpawnPoint(Vector3 position);
    Vector3 NextRandomSpawnPoint();
}

public class LinearSpawnPoint : ISpawnPoint
{
    private List<Vector3> spawnPoints;
    private int currentIndex = 0;
    
    public LinearSpawnPoint(List<Transform> spawnPoints)
    {
        this.spawnPoints = new List<Vector3>();
        foreach (var spawnPoint in spawnPoints)
        {
            this.spawnPoints.Add(spawnPoint.position);
        }
    }
    
    public LinearSpawnPoint(List<Vector3> spawnPoints)
    {
        this.spawnPoints = new List<Vector3>(spawnPoints);
    }
    
    public LinearSpawnPoint(Vector3[] spawnPoints)
    {
        this.spawnPoints = new List<Vector3>(spawnPoints);
    }

    public Vector3 NextSpawnPoint()
    {
        Vector3 spawnPoint = spawnPoints[currentIndex];
        currentIndex = (currentIndex + 1) % spawnPoints.Count;
        return spawnPoint;
    }
    
    public Vector3 NextSpawnPoint(int index) => spawnPoints[index];
    
    public Vector3 NextRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, spawnPoints.Count);
        return spawnPoints[randomIndex];
    }
    
    public Vector3 ClosestSpawnPoint(Vector3 position)
    {
        Vector3 closestSpawnPoint = spawnPoints[0];
        float closestDistance = Vector3.Distance(position, closestSpawnPoint);
        int closestIndex = 0;
        for (int i = 1; i < spawnPoints.Count; i++)
        {
            float distance = Vector3.Distance(position, spawnPoints[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSpawnPoint = spawnPoints[i];
                closestIndex = i;
            }
        }
        currentIndex = (closestIndex + 1) % spawnPoints.Count;
        return closestSpawnPoint;
    }
}