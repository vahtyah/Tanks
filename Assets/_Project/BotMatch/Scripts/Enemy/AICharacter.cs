using System;
using System.Collections.Generic;

public class AICharacter : Character
{
    Dictionary<CharacterStates.CharacterAbility, CharacterAbility> abilityList = new();
    protected CharacterStates.CharacterAbility currentAbility;

    public CharacterAbility GetAbility(CharacterStates.CharacterAbility abilityType) { return abilityList[abilityType]; }

    protected override void Initialize()
    {
        base.Initialize();
        foreach (var ability in base.abilities)
        {
            abilityList.Add(ability.AbilityType, ability);
        }
    }

    public void Reset()
    {
        Health.ResetHealth();
        Controller.Reset();
        gameObject.SetActive(true);
    }
}