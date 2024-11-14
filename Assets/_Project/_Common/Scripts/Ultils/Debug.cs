using System;
using Sirenix.OdinInspector;
using UnityEngine;

[IncludeMyAttributes]
[ShowInInspector]
// [ReadOnly]
[TitleGroup("Debugs", order: 1, alignment: TitleAlignments.Split)]
public class Debug : Attribute {}

[IncludeMyAttributes]
[BoxGroup("@$property.Name")]
public class DebugBox : Attribute
{
    public DebugBox(string name) {}
}