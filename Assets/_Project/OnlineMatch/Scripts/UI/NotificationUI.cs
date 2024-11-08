using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private Transform notificationContainer;
    
    private Queue<NotificationElementUI> notificationQueue = new();
    
    public void ShowNotification(string killerName, string victimName)
    {
        var notification = NotificationElementUI.Create(notificationPrefab, notificationContainer, killerName, victimName);
        notificationQueue.Enqueue(notification);
        if (notificationQueue.Count > 5)
        {
            notificationQueue.Dequeue().DisableNotification();
        }
    }
    
    [Button]
    public void TestNotification()
    {
        ShowNotification("Killer_" + Random.Range(0,10), "Victim_" + Random.Range(0,10));
    }
}
