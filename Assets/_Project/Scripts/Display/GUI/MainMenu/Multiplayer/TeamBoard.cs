using System;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject memberPrefabLeft;
    [SerializeField] private GameObject memberPrefabRight;
    [SerializeField] private TextMeshProUGUI teamName;
    [SerializeField] private Image teamBackground;
    Dictionary<Player, PlayerElement> playerEntries = new();


    private void Initialize(Team team)
    {
        teamName.text = team.TeamType.ToString();
        var color = team.TeamColor;
        color.a = 0.25f;
        teamBackground.color = color;
    }

    public void AddMember(Player player, bool isRight = false)
    {
        PunManager.Instance.AddPlayerDisPlayInRoom(player, isRight ? memberPrefabRight : memberPrefabLeft, memberContainer);
        var playerElement = PunManager.Instance.GetPlayerElement(player.ActorNumber);
        playerEntries.Add(player, playerElement);
    }

    public void RemoveMember(Player player)
    {
        if (!playerEntries.Remove(player, out var playerEntry)) return;
        PunManager.Instance.RemovePlayerDisplayInRoom(player.ActorNumber);
    }
}