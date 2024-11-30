using Unity.MLAgents;
using UnityEngine;

public class AgentController : Agent
{
    [SerializeField] private Transform target;
    // [SerializeField] private float moveSpeed = 4f;
    
    private Rigidbody rb;
    
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
        target.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
    }
    
    
}
