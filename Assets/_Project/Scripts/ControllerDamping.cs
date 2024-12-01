using UnityEngine;

public class ControllerDamping : MonoBehaviour
{
    public float moveSpeed = 5000;
    private Rigidbody tankRigidbody;
    public float damping = 0.9f;

    Vector3 currentDirection;
    private Vector3 _lastMovement;
    private Quaternion _tmpRotation;

    void Start() { tankRigidbody = GetComponent<Rigidbody>(); }

    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized *
                       (moveSpeed * Time.deltaTime);
        if (move.magnitude > 0)
        {
            currentDirection = move.normalized;
        }
        // else
        // {
        //     tankRigidbody.velocity = Vector3.Lerp(tankRigidbody.velocity, Vector3.zero, Time.deltaTime * damping);
        // }

        tankRigidbody.AddForce(move, ForceMode.VelocityChange);
        RotateToFaceMovementDirection();
    }

    void RotateToFaceMovementDirection()
    {
        if (currentDirection.sqrMagnitude >= 0.25f)
        {
            _lastMovement = currentDirection;
        }

        if (_lastMovement != Vector3.zero)
        {
            _tmpRotation = Quaternion.LookRotation(_lastMovement);
            transform.rotation = Quaternion.Slerp(transform.rotation, _tmpRotation, Time.deltaTime * 10);
        }
    }
    
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Velocity: {tankRigidbody.velocity}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Acceleration: {tankRigidbody.velocity.magnitude}");
        // GUI.Label(new Rect(10, 50, 300, 20), $"Deceleration: {}");
    }
}