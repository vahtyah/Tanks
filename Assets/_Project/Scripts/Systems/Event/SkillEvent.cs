public struct SkillEvent
{
    public WeaponState WeaponState;
    public float Param;
    
    public SkillEvent(WeaponState state, float param)
    {
        WeaponState = state;
        Param = param;
    }
    
    public static void TriggerEvent(WeaponState state, float param)
    {
        EventManager.TriggerEvent(new SkillEvent(state, param));
    }
}