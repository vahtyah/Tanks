using System;
using UnityEngine;
using UnityEngine.AI;

public class AgentNavMesh : MonoBehaviour
{
    public int maxHealth = 10;
    public int teamId;
    public LayerMask enemyLayer;
    public Transform weaponModel;
    public float shootRange = 10f;
    private NavMeshAgent agent;
    private Transform target;

    private Collider[] hitColliders = new Collider[10];
    protected CharacterAgentNavMeshHandleWeapon weapon;

    protected Vector3 startPosition;

    private int health;
    public int Health 
    {
        get => health;
        set
        {
            health = value;
            if (health <= 0)
            {
                GameNavMeshController.Instance.OnAgentDeath(this);
            }
        }
    }

    protected virtual void Awake()
    {
        health = maxHealth;
    }

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        target = EnemyPathPoint.Instance.GetRandomPathPoint();
        weapon = GetComponent<CharacterAgentNavMeshHandleWeapon>();
        startPosition = transform.position;
    }

    private void Update()
    {
        ControlTank();
    }

    public virtual void ControlTank()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);

            // Lấy hướng thực tế agent đang di chuyển
            Vector3 direction = agent.velocity;

            if (direction.sqrMagnitude > 0.1f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }

        if (Vector3.Distance(transform.position, target.position) < 4f)
        {
            // Perform shooting action here
            target = EnemyPathPoint.Instance.GetRandomPathPoint();
        }

        var size = Physics.OverlapSphereNonAlloc(transform.position, shootRange, hitColliders, enemyLayer);
        if (size > 0)
        {
            ConsoleProDebug.Watch("Size", size.ToString());
            for (int i = 0; i < size; i++)
            {
                if(Physics.Raycast(transform.position.Add(y: 1f), (hitColliders[i].transform.position - transform.position).normalized, out RaycastHit hit))
                {
                    Debug.DrawLine(transform.position.Add(y: 1f), hit.point, Color.red);
                    if (!hit.collider.CompareTag("Player 1"))
                    {
                        Debug.Log("Obstacle");
                        continue;
                    }
                }
                RotateWeapon(hitColliders[i].transform.position - transform.position, i);
            }
        }
        hitColliders = new Collider[10];
    }
    

    public void RotateWeapon(Vector3 dir, int index)
    {
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            weaponModel.transform.rotation =
                Quaternion.Slerp(weaponModel.transform.rotation, targetRotation, Time.deltaTime * 5);
            
            if (CanShoot(hitColliders[index].transform.position))
            {
                weapon.UseWeapon();
            }
        }
    }

    public bool CanShoot(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - weaponModel.position).normalized;
        // Check if the target is within the field of view of the weapon
        float angleToTarget = Vector3.Angle(weaponModel.forward, directionToTarget);
        return angleToTarget < 10f; // Adjust the angle as needed
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }

    public int GetTeamId()
    {
        return teamId;
    }

    public void ResetPos()
    {
        transform.position = startPosition;
        target = EnemyPathPoint.Instance.GetRandomPathPoint();
        gameObject.SetActive(true);
        health = maxHealth;
    }
}