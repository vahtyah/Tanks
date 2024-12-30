using UnityEngine;

public class RoomOtherSettings : MultiOptionRoomSetting
{
    public override void Initialize()
    {
        cardOptionSelector.Initialize(GetValues(), GetIndex(), SetIndex);
    }

    public override string GetSettingName() => "Settings";

    public override string GetSettingDescription() => "Select the settings you want to play";

    public override void LoadSetting()
    {
        
    }

    public override void SaveSetting()
    {
        var detail = cardOptionSelector as DetailRoomOptionSelector;
        
        if (detail == null)
        {
            Debug.LogError("RoomOtherSettings:SaveSetting: Detail is null");
            return;
        }
        
        uiMultiplayerController.SetRoomName(detail.GetRoomName());
        uiMultiplayerController.SetTeamSize(detail.GetNumberOfTeams());
        uiMultiplayerController.SetPlayerSize(detail.GetNumberOfMaxPlayers());
        uiMultiplayerController.SetCreateBots(detail.IsCreateBots());
    }
}
