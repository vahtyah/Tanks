public abstract  class EnemyState : IState
{
    protected AIController controller;
    protected AICharacter character;

    protected EnemyState(AIController controller, AICharacter character)
    {
        this.controller = controller;
        this.character = character;
    }
    public virtual void OnEnter() { }

    public virtual void OnUpdate() { }

    public virtual void OnFixedUpdate() { }

    public virtual void OnExit() { }
}

public enum EnemyStateType
{
    Patrol,
    Chase,
    Shoot,
    Dead
}