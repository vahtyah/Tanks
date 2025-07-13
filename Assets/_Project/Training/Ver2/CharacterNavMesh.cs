using System;
using UnityEngine;

public class CharacterNavMesh : AgentNavMesh
{
    private Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
        weapon = GetComponent<CharacterAgentNavMeshHandleWeapon>();
        startPosition = transform.position;
    }

    public override void ControlTank()
    {
        var dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized *(200f * Time.deltaTime);
        rb.AddForce(dir, ForceMode.VelocityChange);
        
        if (dir.sqrMagnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
        
        var aimDir = GetAimDirection();
        if (aimDir != Vector3.zero)
        {
            Quaternion targetWeaponRotation = Quaternion.LookRotation(aimDir);
            weaponModel.transform.rotation = Quaternion.Slerp(weaponModel.transform.rotation, targetWeaponRotation, Time.deltaTime * 5);
        }
        
        if (Input.GetButton("Fire1"))
        {
            weapon.UseWeapon();
        }
    }

    public Vector3 GetAimDirection()
    {
        var mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        var direction = (mouseWorldPosition - transform.position).With(y: 0);
        return direction;
    }

    public void OnEnable()
    {
        AgentCameraController.Instance.SwitchToTargetCamera(true);
    }
    
    public void OnDisable()
    {
        AgentCameraController.Instance.SwitchToTargetCamera(false);
    }
}