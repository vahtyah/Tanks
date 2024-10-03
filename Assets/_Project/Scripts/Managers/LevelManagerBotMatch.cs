using UnityEngine;

public class LevelManagerBotMatch : LevelManager
{
    protected override void PreInitialization()
    {
        for (int i = 0; i < PlayerNumber; i++)
        {
            characters.Add(players[i].GetComponent<PlayerCharacter>());
        }
    }
    
    protected override void Initialization()
    {
        
    }

    protected override void PlayerDead(PlayerCharacter character)
    {
        if (character == null) return;
        winner = character;
        StartCoroutine(GameOver());
    }
}