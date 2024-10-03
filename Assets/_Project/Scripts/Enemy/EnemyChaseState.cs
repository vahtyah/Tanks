using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyState
{
    private CharacterAbility moveAbility;
    private CharacterAbility orientationAbility;
    private CharacterAbility fireAbility;
    private Vector3[] pathCorners = Array.Empty<Vector3>();

    private Vector3 lastPlayerPosition;

    public EnemyChaseState(AIController controller, AICharacter character) : base(controller, character)
    {
        moveAbility = controller.character.GetAbility(CharacterStates.CharacterAbility.Move);
        orientationAbility = controller.character.GetAbility(CharacterStates.CharacterAbility.Orientation);
        fireAbility = controller.character.GetAbility(CharacterStates.CharacterAbility.Fire);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        controller.currentDirection = Vector3.zero;
        CalculatePathToPlayer();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (pathCorners.Length <= 2)
        {
            Vector3 playerPosition = controller.playerDetector.player.transform.position;
            Vector3 currentPosition = controller.transform.position;
            Vector3 direction = (playerPosition - currentPosition).With(y: 0);
            controller.currentDirection = direction;
            CalculatePathToPlayer();
        }
        fireAbility.ProcessAbility();
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        moveAbility.FixedProcessAbility();
        orientationAbility.FixedProcessAbility();
    }

    private void CalculatePathToPlayer()
    {
        NavMeshPath navPath = new NavMeshPath();
        Vector3 destination = controller.playerDetector.player.transform.position;
        pathCorners = NavMesh.CalculatePath(controller.transform.position, destination, NavMesh.AllAreas, navPath) ? navPath.corners : Array.Empty<Vector3>();
    }
}