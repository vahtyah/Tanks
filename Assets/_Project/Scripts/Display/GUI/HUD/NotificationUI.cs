using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class NotificationUI : MonoBehaviour, IEventListener<InGameEvent>
{
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private Transform notificationContainer;
    
    private Queue<NotificationElementUI> notificationQueue = new();

    private void ShowNotification(string killerName, string victimName)
    {
        var notification = NotificationElementUI.Create(notificationPrefab, notificationContainer, killerName, victimName);
        notificationQueue.Enqueue(notification);
        if (notificationQueue.Count > 5)
        {
            notificationQueue.Dequeue().HideNotification();
        }
    }
    
    [Button]
    public void TestNotification()
    {
        ShowNotification("Killer_" + Random.Range(0,10), "Victim_" + Random.Range(0,10));
    }

    public void OnEvent(InGameEvent e)
    {
        if (e.EventType == InGameEventType.SomeoneDied)
        {
            ShowNotification(e.killer.PhotonView.Owner.NickName, e.victim.PhotonView.Owner.NickName);
        }
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
