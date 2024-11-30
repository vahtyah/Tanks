using System;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class TrainingController : MonoBehaviour
{
    public List<CharacterAgent> team1Players = new List<CharacterAgent>();
    public List<CharacterAgent> team2Players = new List<CharacterAgent>();
    
    public Transform team1HomeBase;
    public Transform team2HomeBase;
    
    private SimpleMultiAgentGroup team1Group;
    private SimpleMultiAgentGroup team2Group;

    public List<CharacterAgent> Team1PlayersAlive;
    public List<CharacterAgent> Team2PlayersAlive;

    private Timer gameTimer;

    private int MaxEnvironmentSteps = 10000;
    private int ResetTimer = 0;

    private void Start()
    {
        team1Group = new SimpleMultiAgentGroup();
        team2Group = new SimpleMultiAgentGroup();
        // gameTimer = Timer.Register(new LocalTimer(100))
        //     .OnComplete(() =>
        //     {
        //         team1Group.GroupEpisodeInterrupted();
        //         team2Group.GroupEpisodeInterrupted();
        //         ResetScene();
        //     });
        ResetScene();
    }

    private void FixedUpdate()
    {
        ResetTimer++;
        if (ResetTimer >= MaxEnvironmentSteps)
        {
            team1Group.GroupEpisodeInterrupted();
            team2Group.GroupEpisodeInterrupted();
            ResetScene();
        }
    }


    public void SomeoneDied(CharacterAgent agent)
    {
        Debug.Log("Someone died");
        if (team1Players.Contains(agent))
        {
            Team1PlayersAlive.Remove(agent);
        }
        else if (team2Players.Contains(agent))
        {
            Team2PlayersAlive.Remove(agent);
        }
        agent.gameObject.SetActive(false);

        if (Team1PlayersAlive.Count == 0)
        {
            team2Group.AddGroupReward(2 - (ResetTimer / MaxEnvironmentSteps));
            team1Group.AddGroupReward(-1);
        }
        else if (Team2PlayersAlive.Count == 0)
        {
            team1Group.AddGroupReward(2 - (ResetTimer / MaxEnvironmentSteps));
            team2Group.AddGroupReward(-1);
        }

        if (Team1PlayersAlive.Count == 0 || Team2PlayersAlive.Count == 0)
        {
            team1Group.EndGroupEpisode();
            team2Group.EndGroupEpisode();
            ResetScene();
        }
    }

    public void ResetScene()
    {
        // gameTimer.Cancel();
        ResetTimer = 0;
        Team2PlayersAlive.Clear();
        Team1PlayersAlive.Clear();
        foreach (var player in team1Players)
        {
            player.ResetAgent();
            team1Group.RegisterAgent(player);
            Team1PlayersAlive.Add(player);
        }

        foreach (var player in team2Players)
        {
            player.ResetAgent();
            team2Group.RegisterAgent(player);
            Team2PlayersAlive.Add(player);
        }

        UnityEngine.Debug.Log(
            $"Team 1: {team1Group.GetRegisteredAgents().Count} players, Team 2: {team2Group.GetRegisteredAgents().Count} players");

        // gameTimer.ReStart();
    }
}