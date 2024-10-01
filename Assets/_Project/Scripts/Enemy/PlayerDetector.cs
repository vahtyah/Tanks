using System;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] float detectionAngle = 60f; // Cone in front of enemy
    [SerializeField] float detectionRadius = 10f; // Large circle around enemy
    [SerializeField] float innerDetectionRadius = 5f; // Small circle around enemy
    [SerializeField] float detectionCooldown = 1f; // Time between detections
    [SerializeField] float attackRange = 2f; // Distance from enemy to player to attack
    public Character player;

    public bool CanDetectPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Vector3 directionToPlayer = collider.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, directionToPlayer);
                if (angle < detectionAngle)
                {
                    if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, detectionRadius))
                    {
                        if (hit.collider == collider)
                        {
                            player = collider.GetComponent<Character>();
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(transform.position, innerDetectionRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, detectionAngle, 0) * transform.forward * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, -detectionAngle, 0) * transform.forward * detectionRadius);
    }
}
