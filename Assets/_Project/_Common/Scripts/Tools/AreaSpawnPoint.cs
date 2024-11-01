using System.Collections.Generic;
using UnityEngine;

public class AreaSpawnPoint : ISpawnPoint
{
    private Collider spawnArea;
    private List<Vector3> spawnPoints;
    private List<Vector3> unusedPoint;
    private int currentIndex = 0;

    public AreaSpawnPoint(Collider spawnArea)
    {
        this.spawnArea = spawnArea;
        spawnPoints = new List<Vector3>();
        foreach (Transform spawnPoint in spawnArea.transform)
        {
            spawnPoints.Add(spawnPoint.position);
        }
    }
    
    private Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            spawnArea.transform.position.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
    
    public Vector3 NextSpawnPoint()
    {
        if (spawnPoints.Count == 0)
            return RandomPointInBounds(spawnArea.bounds);
        Vector3 spawnPoint = spawnPoints[currentIndex];
        currentIndex = (currentIndex + 1) % spawnPoints.Count;
        return spawnPoint;
    }

    public Vector3 ClosestSpawnPoint(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    public Vector3 NextRandomSpawnPoint()
    {
        if(spawnPoints.Count == 0)
            return RandomPointInBounds(spawnArea.bounds);
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
        if(spawnPoints.Count == 0)
            return RandomPointInBounds(spawnArea.bounds);
        int spawnIndex = index % spawnPoints.Count;
        return spawnPoints[spawnIndex];
    }
}