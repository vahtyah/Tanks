﻿using UnityEngine;

public class CharacterPause : CharacterAbility
{
    public override void ProcessAbility()
    {
        base.ProcessAbility();
        HandleInput();
    }

    protected override void HandleInput()
    {
        if (controller.isButtonPauseDown)
        {
            TriggerPause();
        }
    }

    private void TriggerPause()
    {
        GameEvent.Trigger(GameEventType.TogglePause);
    }
}
