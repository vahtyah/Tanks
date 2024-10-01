public class EnemyShootState : EnemyState
{
    private CharacterAbility fireAbility;

    public EnemyShootState(AIController controller, AICharacter character) : base(controller, character)
    {
        fireAbility = controller.character.GetAbility(CharacterStates.CharacterAbility.Fire);
    }
    
    
}