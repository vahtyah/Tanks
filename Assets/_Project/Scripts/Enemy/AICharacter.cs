using System.Collections.Generic;

public class AICharacter : Character
{
    Dictionary<CharacterStates.CharacterAbility, CharacterAbility> abilities = new();
    protected CharacterStates.CharacterAbility currentAbility;
    
    public CharacterAbility GetAbility(CharacterStates.CharacterAbility abilityType)
    {
        return abilities[abilityType];
    }

    protected override void Initialization()
    {
        base.Initialization();
        foreach (var ability in characterAbilities)
        {
            abilities.Add(ability.AbilityType, ability);
        }
    }
}