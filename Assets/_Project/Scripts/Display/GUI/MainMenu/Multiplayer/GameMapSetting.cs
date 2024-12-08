using System.Collections.Generic;
using UnityEngine;

public class GameMapSetting : MultiOptionRoomSetting
{
    public List<GameMapData> Datas;

    public override void Initialize()
    {
        foreach (var gameMap in Datas)
        {
            AddOption(gameMap);
        }

        SelectOption(Datas[0]);
        cardOptionSelector.Initialize(GetValues(), GetIndex(), SetIndex);
    }

    public override string GetSettingName() => "Game Map";

    public override string GetSettingDescription() => "Select the map you want to play";

    public override void LoadSetting()
    {
        throw new System.NotImplementedException();
    }

    public override void SaveSetting()
    {
        // var option = GetValues()[GetIndex()] as GameMapData;
        // if (option != null) Photon.Pun.PhotonNetwork.CurrentRoom.SetGameMap(option.GameMap);
        // else Debug.LogError("GameMapSetting:SaveSetting: Option is null");
    }
}