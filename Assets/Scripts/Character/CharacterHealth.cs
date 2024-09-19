using UnityEngine;

public class CharacterHealth : Health
{
    protected override void OnDeath()
    {
        character.conditionState.ChangeState(CharacterStates.CharacterCondition.Dead);
        Event.Trigger(EventType.PlayerDeath, GetComponent<Character>());
        gameObject.SetActive(false);
    }
}
