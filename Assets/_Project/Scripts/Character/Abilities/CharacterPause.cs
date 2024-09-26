using UnityEngine;

public class CharacterPause : CharacterAbility
{
    public override void ProcessAbility()
    {
        base.ProcessAbility();
        HandleInput();
    }

    protected override void HandleInput()
    {
        if (characterInput.isButtonPauseDown)
        {
            TriggerPause();
        }
    }

    private void TriggerPause()
    {
        Event.Trigger(EventType.TogglePause, null);        
    }
}
