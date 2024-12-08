using UnityEngine;

[CreateAssetMenu(fileName = "GameMode", menuName = "RoomSettings/GameMode", order = 0)]
public class GameModeData : CardData
{
    public GameMode GameMode;
}

public enum GameMap
{
    Map1,
    Map2,
    Map3
}