using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameNavMeshController: Singleton<GameNavMeshController>
{
    public Image winnerPanel;
    public TextMeshProUGUI winnerText;
    public Color blueTeamColor;
    public Color redTeamColor;
    
    public ParticleSystem explosionEffect;
    public AgentNavMesh[] blueTeam;
    public AgentNavMesh[] redTeam;
    
    public int blueTeamAliveCount = 4;
    public int redTeamAliveCount = 4;

    protected override void Awake()
    {
        base.Awake();
        blueTeamAliveCount = blueTeam.Length;
        redTeamAliveCount = redTeam.Length;
    }

    public void OnAgentDeath(AgentNavMesh agent)
    {
        Pool.Spawn(explosionEffect.gameObject, agent.transform.position, explosionEffect.transform.rotation).GetComponent<ParticleSystem>().Play();
        agent.gameObject.SetActive(false);
        if (agent.teamId == 0)
        {
            blueTeamAliveCount--;
        }
        else if (agent.teamId == 1)
        {
            redTeamAliveCount--;
        }

        if(blueTeamAliveCount <= 0 || redTeamAliveCount <= 0)
        {
            StartCoroutine(WaitForGameOver());
        }
    }
    
    public void OnGameOver()
    {
        winnerPanel.gameObject.SetActive(true);
        if (blueTeamAliveCount < redTeamAliveCount)
        {
            winnerText.text = "Blue Team Wins!";
            winnerPanel.color = blueTeamColor;
        }
        else if (redTeamAliveCount < blueTeamAliveCount)
        {
            winnerText.text = "Red Team Wins!";
            winnerPanel.color = redTeamColor;
        }
        else
        {
            winnerText.text = "It's a Draw!";
            winnerPanel.color = Color.white;
        }
    }
    
    IEnumerator WaitForGameOver()
    {
        OnGameOver();
        yield return new WaitForSeconds(2f);
        winnerPanel.gameObject.SetActive(false);
        ResetGame();
    }

    private void ResetGame()
    {
        foreach (var agent in blueTeam)
        {
            agent.ResetPos();
        }
        
        foreach (var agent in redTeam)
        {
            agent.ResetPos();
        }
        
        blueTeamAliveCount = blueTeam.Length;
        redTeamAliveCount = redTeam.Length;
    }
}
