using System;
using Unity.MLAgents;
using UnityEngine;

public class CharacterAgentMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject weaponModel;
    

    private Rigidbody rb;
    public Vector3 DirNormalized { get; private set; }
    private Vector3 lastMovementDirection;
    private Quaternion modelRotation;
    private CharacterAgent agent;

    private Vector3 lastDir = Vector3.zero;
    public Vector3 Velocity => rb.velocity;

    // private void OnCollisionEnter(Collision other)
    // {
    //     if (other.gameObject.CompareTag("Obstacle"))
    //     {
    //         agent.AddReward(-1f);
    //         agent.EndEpisode();
    //     }
    //     // else if (other.gameObject.CompareTag("Player"))
    //     // {
    //     //     agent.AddReward(+1f);
    //     //     other.gameObject.GetComponent<CharacterAgent>().AgentDie();
    //     // }
    //     if(other.gameObject.CompareTag("Enemy"))
    //     {
    //         agent.AddReward(+5f);
    //         agent.EndEpisode();
    //     }
    // }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<CharacterAgent>();
    }

    public void Move(Vector3 dir)
    {
        DirNormalized = dir.With(y: 0).normalized;
        rb.AddForce(DirNormalized * speed * Time.deltaTime, ForceMode.VelocityChange);
        
        // var dotProduct = Vector3.Dot(lastDir, DirNormalized);
        // if (dotProduct > .9f)
        //     agent.AddReward(.005f);
        // lastDir = DirNormalized;
    }

    private void Rotate()
    {
        if (DirNormalized.sqrMagnitude >= .5f)
        {
            lastMovementDirection = DirNormalized;
        }
        
        if (lastMovementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lastMovementDirection);
            modelRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);
        }
        
        transform.rotation = modelRotation;
    }

    public void RotateWeapon(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            weaponModel.transform.rotation = Quaternion.Slerp(weaponModel.transform.rotation, targetRotation, Time.deltaTime * 5);
        }
    }
    
    public float GetOrientationWeaponRelative()
    {
        return Vector3.Angle(weaponModel.transform.forward, transform.forward);
    }
    
    public Vector3 GetWeaponDirection()
    {
        return weaponModel.transform.forward;
    }

    private void Update()
    {
        Rotate();
    }
}