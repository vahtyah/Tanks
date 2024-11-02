using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class Flag : MonoBehaviour
{
    [SerializeField] private Renderer rd;
    public Renderer Rd => rd;
    public TeamType Team { get; private set; }
    public TeamType TeamCaptured { get; private set; } = TeamType.None;
    private Vector3 initialPosition;
    public float respawnDuration = 5f;
    private Timer respawnTimer;

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        initialPosition = transform.position;
        respawnTimer = Timer.Register(respawnDuration)
            .OnComplete(Return)
            .AutoDestroyWhenOwnerDisappear(this);
    }

    public void Initialize(TeamType team, Material material)
    {
        Team = team;
        rd.material = material;
    }

    public void Reset()
    {
        TeamCaptured = TeamType.None;
    }

    public void Capture(TeamType team)
    {
        if (TeamCaptured == TeamType.None)
        {
            TeamCaptured = team;
            gameObject.SetActive(false);
        }
    }

    public void Release(Vector3 position)
    {
        TeamCaptured = TeamType.None;
        transform.position = position.With(y:initialPosition.y);
        gameObject.SetActive(true);
        respawnTimer.Reset();
    }

    public void Return()
    {
        TeamCaptured = TeamType.None;
        transform.position = initialPosition;
        gameObject.SetActive(true);
    }
}