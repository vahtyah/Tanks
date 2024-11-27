using System;
using Sirenix.OdinInspector;


[IncludeMyAttributes]
[ShowInInspector]
// [ReadOnly]
[TitleGroup("Debugs", order: 1, alignment: TitleAlignments.Split)]
public class Log : Attribute
{
}