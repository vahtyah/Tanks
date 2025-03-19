using UnityEngine;

public static class ColorDebug
{
    public static void ColorLog(object message, Color color = default)
    {
        Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>");
    }
    
    public static void RedLog(object message)
    {
        ColorLog(message, Color.red);
    }
    
    public static void GreenLog(object message)
    {
        ColorLog(message, Color.green);
    }
    
    public static void YellowLog(object message)
    {
        ColorLog(message, Color.yellow);
    }
}