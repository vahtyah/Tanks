using TMPro;
using UnityEngine;

public class KillNotificationView : NotificationView
{
    [SerializeField] private TextMeshProUGUI killerNameText;
    [SerializeField] private TextMeshProUGUI victimNameText;
    
    private void Initialize(string killerName, string victimName)
    {
        killerNameText.text = killerName;
        victimNameText.text = victimName;
    }
}