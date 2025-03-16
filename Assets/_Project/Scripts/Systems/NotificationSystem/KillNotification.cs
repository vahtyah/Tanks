public class KillNotification : Notification
{
    public string KillerName { get; private set; }
    public string VictimName { get; private set; }
    
    public KillNotification(string killerName, string victimName)
    {
        KillerName = killerName;
        VictimName = victimName;
    }
    
    public override void OnDisplay()
    {
        
    }

    public override void OnHide()
    {
    }
}