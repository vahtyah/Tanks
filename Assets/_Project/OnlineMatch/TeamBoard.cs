using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class TeamBoard : MonoBehaviour
{
    public static TeamBoard Create(GameObject prefab, Transform parent, Team team = null)
    {
        var instance = Instantiate(prefab, parent).GetComponent<TeamBoard>();
        if (team != null)
            instance.Initialize(team);
        return instance;
    }

    [SerializeField] private Transform memberContainer;
    [SerializeField] private GameObject memberPrefab;
    [SerializeField] private TextMeshProUGUI teamName;
    Dictionary<Player, PlayerElement> playerEntries = new();

    private void Initialize(Team team)
    {
        teamName.text = team.TeamType.ToString();
    }

    public void AddMember(Player player)
    {
        PunManager.Instance.AddPlayerDisPlayInRoom(player, memberPrefab, memberContainer);
        var playerElement = PunManager.Instance.GetPlayerElement(player.ActorNumber);
        playerEntries.Add(player, playerElement);
    }

    public void RemoveMember(Player player)
    {
        if (!playerEntries.Remove(player, out var playerEntry)) return;
        PunManager.Instance.RemovePlayerDisplayInRoom(player.ActorNumber);
    }
}