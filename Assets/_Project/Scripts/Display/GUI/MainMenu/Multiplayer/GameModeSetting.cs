using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameModeSetting : MultiOptionRoomSetting
{
    public List<GameModeData> Datas;
    [SerializeField] private Animator selectAmountAnimator;


    public override void Initialize()
    {
        foreach (var cardData in Datas)
        {
            AddOption(cardData);
        }

        SelectOption(Datas[0]);
        cardOptionSelector.Initialize(GetValues(), GetIndex(), SelectOption);
        HandleSetting(Datas[0]);
    }

    private void HandleSetting(GameModeData data)
    {
        if (data.GameMode == GameMode.Deathmatch)
        {
            selectAmountAnimator.Play("MaxPlayers");
        }
        else
        {
            selectAmountAnimator.Play("Room");
        }
    }

    private void SelectOption(int index)
    {
        SetIndex(index);
        var option = GetValues()[index] as GameModeData;
        if (option == null)
        {
            return;
        }

        HandleSetting(option);
    }

    public override string GetSettingName() => "Game Mode";

    public override string GetSettingDescription() => "Select the game mode you want to play";

    public override void LoadSetting()
    {
        throw new System.NotImplementedException();
    }

    public override void SaveSetting()
    {
        var option = GetValues()[GetIndex()] as GameModeData;
        Debug.Log("GameModeSetting:SaveSetting: " + option.GameMode);
        if (option != null) uiMultiplayerController.SetGameMode(option.GameMode);
        else Debug.LogError("GameModeSetting:SaveSetting: Option is null");
    }
}