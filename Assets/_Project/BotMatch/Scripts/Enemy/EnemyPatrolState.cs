using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyState
{
    private CharacterAbility movementAbility;
    private CharacterAbility orientationAbility;
    private int currentCornerIndex = 0;
    private Vector3[] pathCorners = Array.Empty<Vector3>();
    //TODO: Sử dụng Dynamic Programming để lưu lại path đã tính toán 
    // private Dictionary<Vector3, Vector3[]> pathCache = new Dictionary<Vector3, Vector3[]>();
    
    //TODO: Sử dụng OnTriggerEnter để custom ObstacleAvoidance

    public EnemyPatrolState(AIController controller, AICharacter character) : base(controller, character)
    {
        movementAbility = controller.character.GetAbility(CharacterStates.CharacterAbility.Move);
        orientationAbility = controller.character.GetAbility(CharacterStates.CharacterAbility.Orientation);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        CalculatePathToTarget();

    }

    public override void OnUpdate()
    {
        base.OnUpdate(); 
        if (pathCorners.Length == 0)
            return;

        Vector3 currentPosition = controller.transform.position;
        Vector3 targetPosition = pathCorners[currentCornerIndex];

        Vector3 direction = (targetPosition - currentPosition).With(y: 0);
        controller.currentDirection = direction;

        if (direction.sqrMagnitude < 1f * 1f)
        {
            currentCornerIndex++;
            if (currentCornerIndex >= pathCorners.Length)
                CalculatePathToTarget();
        }
    }
    
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        movementAbility.FixedProcessAbility();
        orientationAbility.FixedProcessAbility();
    }

    private void CalculatePathToTarget()
    {
        NavMeshPath navPath = new NavMeshPath();
        Vector3 destination = controller.path.NextRandomSpawnPoint();

        if (NavMesh.CalculatePath(controller.transform.position, destination, NavMesh.AllAreas, navPath))
        {
            pathCorners = navPath.corners;
            currentCornerIndex = 0;
        }
        else
            pathCorners = Array.Empty<Vector3>();
    }
}