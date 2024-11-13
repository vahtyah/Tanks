using UnityEngine;

public class EnemyDieState : EnemyState
{
    private Timer timer;
    public EnemyDieState(AIController controller, AICharacter character) : base(controller, character)
    {
        timer = Timer.Register(5).OnComplete(character.Reset);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        timer.ReStart();
    }
}