using System;
using Sirenix.OdinInspector;
using UnityEngine;


[IncludeMyAttributes]
[ShowInInspector]
// [ReadOnly]
[TitleGroup("Debugs", order: 1, alignment: TitleAlignments.Split)]
public class Log : Attribute
{
}

[IncludeMyAttributes]
[BoxGroup("Settings")]
[SerializeField]
public class Settings : Attribute
{

}