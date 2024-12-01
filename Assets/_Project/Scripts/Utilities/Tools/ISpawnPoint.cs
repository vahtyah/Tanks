using System.Collections.Generic;
using UnityEngine;

public interface ISpawnPoint
{
    Vector3 NextSpawnPoint();
    Vector3 ClosestSpawnPoint(Vector3 position);
    Vector3 NextRandomSpawnPoint();
    Vector3 NextSpawnPointByIndex(int index);
}

public class LinearSpawnPoint : ISpawnPoint
{
    private List<Vector3> spawnPoints;
    private List<Vector3> unusedPoint;
    private int currentIndex = 0;
    
    public LinearSpawnPoint(List<Vector3> spawnPoints)
    {
        this.spawnPoints = spawnPoints;
        unusedPoint = new List<Vector3>(spawnPoints);
    }
    
    public LinearSpawnPoint(List<Transform> spawnPoints)
    {
        this.spawnPoints = new List<Vector3>();
        foreach (Transform spawnPoint in spawnPoints)
        {
            this.spawnPoints.Add(spawnPoint.position);
        }
        unusedPoint = new List<Vector3>(this.spawnPoints);
    }

    public Vector3 NextSpawnPoint()
    {
        Vector3 spawnPoint = spawnPoints[currentIndex];
        currentIndex = (currentIndex + 1) % spawnPoints.Count;
        return spawnPoint;
    }
    
    public Vector3 NextRandomSpawnPoint()
    {
        if (unusedPoint.Count == 0)
        {
            unusedPoint = new List<Vector3>(spawnPoints);
        }
        int randomIndex = Random.Range(0, unusedPoint.Count);
        Vector3 spawnPoint = unusedPoint[randomIndex];
        unusedPoint.RemoveAt(randomIndex);
        return spawnPoint;
    }

    public Vector3 NextSpawnPointByIndex(int index)
    {
        int spawnIndex = index % spawnPoints.Count;
        return spawnPoints[spawnIndex];
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