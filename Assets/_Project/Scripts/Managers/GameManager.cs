using System;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>, IEventListener<Event>
{
    public int NumberOfPlayers = 2;

    private void OnEnable() { this.StartListening(); }

    private void OnDisable() { this.StopListening(); }
    public void OnEvent(Event e) {  }
}