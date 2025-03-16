using UnityEngine;

public class GeneralNotification : Notification
{
    public string Title { get; private set; }
    public string Destination { get; private set; }

    public override void OnDisplay()
    {
    }

    public override void OnHide()
    {
    }
}