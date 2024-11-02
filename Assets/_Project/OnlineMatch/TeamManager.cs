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

    Dictionary<TeamType, Flag> flags = new Dictionary<TeamType, Flag>();
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
            // var flagGO = PhotonNetwork.Instantiate(flagPrefab.name, teamResources[i].SpawnArea.position, flagPrefab.transform.rotation);
            var flagGO = Pool.Spawn(flagPrefab, teamResources[i].SpawnArea.position, flagPrefab.transform.rotation);
            var flag = flagGO.GetComponent<Flag>();
            flag.Initialize(team[i].TeamType, teamResources[i].FlagMaterial);
            flags.Add(team[i].TeamType, flag);
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

    public Flag GetFlag(TeamType teamType)
    {
        return flags.GetValueOrDefault(teamType, null);
    }
}