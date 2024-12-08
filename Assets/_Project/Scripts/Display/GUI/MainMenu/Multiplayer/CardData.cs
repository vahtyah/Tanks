using Sirenix.OdinInspector;
using UnityEngine;

public class CardData : ScriptableObject
{
    public string Name;
    public Sprite Preview;
    [MultiLineProperty(10)]public string Description;
}