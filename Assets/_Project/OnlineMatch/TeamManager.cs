using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
// ReSharper disable All

[Serializable]
public class TeamResource
{
    public TeamType TeamType;
    public Transform SpawnArea;
    public Material FlagMaterial;
}

public class TeamManager : Singleton<TeamManager>, IEventListener<GameEvent>
{
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private List<TeamResource> teamResources;
    
    public List<Transform> SpawnAreas => teamResources.ConvertAll(x => x.SpawnArea);
    public Transform GetSpawnArea(TeamType teamType) => teamResources.Find(x => x.TeamType == teamType).SpawnArea;

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GamePreStart:
                if (PhotonNetwork.IsMasterClient)
                    GetComponent<PhotonView>().RPC(nameof(SpawnFlags), RpcTarget.AllBuffered);
                break;
        }
    }

    [PunRPC]
    private void SpawnFlags()
    {
        var team = Team.GetTeams();
        for (int i = 0; i < team.Count; i++)
        {
            var flagGO = Pool.Spawn(flagPrefab, teamResources[i].SpawnArea.position, flagPrefab.transform.rotation);
            var flag = flagGO.GetComponent<Flag>();
            flag.Initialize(team[i].TeamType, teamResources[i].FlagMaterial);
        }
    }

    private void OnEnable()
    {
        this.StartListening();
    }

    private void OnDisable()
    {
        this.StopListening();
    }
}