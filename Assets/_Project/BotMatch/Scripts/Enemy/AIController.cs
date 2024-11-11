using System;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour, ICharacterController
{
    private Rigidbody rb;
    public Path path;
    public NavMeshAgent agent;
    public AICharacter character;
    public PlayerDetector playerDetector;

    StateMachine stateMachine;
    public string CurrentState;

    public Vector3 currentDirection;

    private void Awake()
    {
        stateMachine = new StateMachine();
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<AICharacter>();
        path = GetComponent<Path>();
        currentDirection = Vector3.zero;
        playerDetector = GetComponent<PlayerDetector>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        var patrolState = new EnemyPatrolState(this, character);
        var chaseState = new EnemyChaseState(this, character);
        var dieState = new EnemyDieState(this, character);

        stateMachine.At(patrolState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
        stateMachine.At(chaseState, patrolState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
        stateMachine.Any(dieState, ref character.Health.onDeath);
        stateMachine.SetState(patrolState);
    }

    private void Update()
    {
        stateMachine.Update();
        CurrentState = stateMachine.CurrentState.ToString();
    }

    private void FixedUpdate() { stateMachine.FixedUpdate(); }

    public bool isButtonPauseDown { get; }
    public Vector3 GetDirection() { return currentDirection; }

    public bool GetFire() { return true; }
    public bool GetInputSkill()
    {
        return false;
    }

    public Vector3 GetAimDirection()
    {
        // if(playerDetector.player == null)
        //     return Vector3.zero;
        // Vector3 playerPosition = playerDetector.player.transform.position;
        // Vector3 currentPosition = transform.position;
        // Vector3 direction = (playerPosition - currentPosition).With(y: 0);
        // return direction;

        return currentDirection;
    }

    public void Move(Vector3 directionInput) { rb.AddForce(directionInput, ForceMode.VelocityChange); }

    public void Reset() { stateMachine.Reset(); }

    public Rigidbody GetRigidbody()
    {
        return rb;
    }

    public void AddOnTriggerEnter(Action<Collider> action)
    {
        
    }
}