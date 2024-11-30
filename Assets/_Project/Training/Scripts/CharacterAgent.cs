using System;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterAgent : Agent
{
    private TrainingController trainingController;
    private CharacterAgentMovement movement;
    private CharacterAgentHandleWeapon weaponHandler;
    private bool isDecisionStep;
    private Vector3 startingPosition;
    private BehaviorParameters behaviorParameters;
    private BufferSensorComponent bufferSensorComponent;
    
    List<CharacterAgent> enemies = new List<CharacterAgent>();
    [Log] List<CharacterAgent> teammates = new List<CharacterAgent>();
    
    Vector3 homeBasePosition;

    private void Awake()
    {
        startingPosition = transform.position;
        movement = GetComponent<CharacterAgentMovement>();
        weaponHandler = GetComponent<CharacterAgentHandleWeapon>();
        trainingController = GetComponentInParent<TrainingController>();
        behaviorParameters = GetComponent<BehaviorParameters>();
        bufferSensorComponent = GetComponentInChildren<BufferSensorComponent>();
    }

    private void Start()
    {
        if(behaviorParameters.TeamId == 0)
        {
            enemies = trainingController.team2Players;
            teammates = trainingController.team1Players;
        }
        else
        {
            enemies = trainingController.team1Players;
            teammates = trainingController.team2Players;
        }

        Debug.Log(teammates.Count);
    }

    private void FixedUpdate()
    {
        if (StepCount % 5 == 0)
        {
            isDecisionStep = true;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var continuousActions = actions.ContinuousActions;
        var discreteActions = actions.DiscreteActions;

        //Move the agent
        var moveDir = new Vector3(continuousActions[0], 0, continuousActions[1]);
        moveDir = transform.TransformDirection(moveDir);
        movement.Move(moveDir);
        
        
        //Rotate the weapon
        var rotateDir = new Vector3(continuousActions[2], 0, continuousActions[3]);
        movement.RotateWeapon(rotateDir);
        
        var fire = discreteActions[0] == 1;
        if (isDecisionStep && fire)
        {
            isDecisionStep = false;
            weaponHandler.UseWeapon();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // sensor.AddObservation(transform.localPosition);
        // sensor.AddObservation(target.localPosition);
        
        // sensor.AddObservation(movement.GetWeaponDirection());
        sensor.AddObservation(weaponHandler.IsReadyToFire());
        sensor.AddObservation(Vector3.Dot(movement.Velocity, transform.forward));
        sensor.AddObservation(Vector3.Dot(movement.Velocity, transform.right));
        sensor.AddObservation(movement.GetOrientationWeaponRelative());
        
        foreach (var teammate in teammates)
        {
            if (teammate != this && teammate.gameObject.activeInHierarchy)
            {
                bufferSensorComponent.AppendObservation(GetOtherAgentObservation(teammate));
            }
        }
        sensor.AddObservation(trainingController.Team1PlayersAlive.Count);
        sensor.AddObservation(trainingController.Team2PlayersAlive.Count);
    }

    private float[] GetOtherAgentObservation(CharacterAgent teammate)
    {
        var observation = new float[6];
        var relativePosition = transform.InverseTransformPoint(teammate.transform.position);
        observation[0] = relativePosition.x;
        observation[1] = relativePosition.z;
        observation[2] = teammate.weaponHandler.IsReadyToFire() ? 1f : 0f;
        var relativeVelocity = transform.InverseTransformDirection(teammate.movement.Velocity);
        observation[3] = relativeVelocity.x;
        observation[4] = relativeVelocity.z;
        observation[5] = teammate.GetTeamId() == behaviorParameters.TeamId ? 1f : 0f;
        return observation;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        actionsOut.ContinuousActions.Array[0] = Input.GetAxis("Horizontal");
        actionsOut.ContinuousActions.Array[1] = Input.GetAxis("Vertical");

        var mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        var direction = (worldPos - transform.position).With(y: 0);
        actionsOut.ContinuousActions.Array[2] = direction.x;
        actionsOut.ContinuousActions.Array[3] = direction.z;
        
        actionsOut.DiscreteActions.Array[0] = Input.GetMouseButton(0) ? 1 : 0;
    }

    public override void OnEpisodeBegin()
    {
        // ResetAgent();
    }

    public void ResetAgent()
    {
        gameObject.SetActive(true);
        transform.position = startingPosition;
        // transform.localPosition = new Vector3(Random.Range(-20, 20), transform.localPosition.y, Random.Range(-20, 20));
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
    }

    public void AgentDie()
    {
        trainingController.SomeoneDied(this);
    }
    
    public int GetTeamId()
    {
        return behaviorParameters.TeamId;
    }
}