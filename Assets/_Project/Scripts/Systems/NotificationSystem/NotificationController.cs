using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class NotificationController : MonoBehaviour, IEventListener<NotificationEvent>
{
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private GameObject notificationContainer;
    [SerializeField] private int maxVisibleNotifications = 5;

    private Queue<NotificationView> notificationQueue = new();

    public void ShowNotification(string title, string destination)
    {
        var notification =
            GeneralNotificationView.Spawn(notificationPrefab, notificationContainer.transform, title, destination);
        notificationQueue.Enqueue(notification);

        if (notificationQueue.Count > maxVisibleNotifications)
        {
            notificationQueue.Dequeue().HideNotification();
        }
    }
    
    [Button]
    public void TestNotification()
    {
        NotificationEvent.Trigger("Test Title " + Random.Range(0, 10), "Test Destination " + Random.Range(0, 10));
    }

    public void OnEvent(NotificationEvent e)
    {
        ShowNotification(e.Title, e.Destination);
    }

    private void OnEnable()
    {
        this.StartListening();
    }

    private void OnDisable()
    {
        this.StopListening();
    }
}