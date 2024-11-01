using System;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class TeamBoard : MonoBehaviour
{
    public static TeamBoard Create(GameObject prefab, Transform parent, Team team)
    {
        var instance = Instantiate(prefab, parent).GetComponent<TeamBoard>();
        instance.Initialize(team);
        return instance;
    }
    
    [SerializeField] private Transform memberContainer;
    [SerializeField] private GameObject memberPrefab;
    [SerializeField] private TextMeshProUGUI teamName;
    Dictionary<Player, PlayerEntry> playerEntries = new();

    private Team team;
    private void Initialize(Team team)
    {
        this.team = team;
        teamName.text = team.TeamType.ToString();
    }
    
    public void AddMember(Player player)
    {
        var playerEntry = PlayerEntry.Create(memberPrefab, player.NickName, player.ActorNumber, memberContainer);
        LobbyMainMenuPanel.Instance.playerListEntries.Add(player.ActorNumber, playerEntry);
        playerEntries.Add(player, playerEntry);
    }

    public void RemoveMember(Player player)
    {
        if (!playerEntries.TryGetValue(player, out var playerEntry)) return;
        PlayerEntry.Remove(playerEntry);
        playerEntries.Remove(player);
        LobbyMainMenuPanel.Instance.playerListEntries.Remove(player.ActorNumber);
    }
}