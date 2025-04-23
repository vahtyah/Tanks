using UnityEngine;

public struct SkillEvent
{
    public WeaponState WeaponState;
    public float Param;
    public Sprite Icon;
    
    public SkillEvent(WeaponState state, float param, Sprite icon)
    {
        WeaponState = state;
        Param = param;
        Icon = icon;
    }
    
    public static void TriggerEvent(WeaponState state, float param, Sprite icon = null)
    {
        EventManager.TriggerEvent(new SkillEvent(state, param, icon));
    }
}