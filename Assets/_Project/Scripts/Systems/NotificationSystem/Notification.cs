public abstract class Notification
{
    public float displayTime = 5f;
    public abstract void OnDisplay();
    public abstract void OnHide();
}