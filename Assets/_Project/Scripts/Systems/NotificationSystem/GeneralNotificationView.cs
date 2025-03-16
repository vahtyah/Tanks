using System.Collections;
using TMPro;
using UnityEngine;

public class GeneralNotificationView : NotificationView
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI destinationText;

    private Animator anim;
    private string notiIn = "In";
    private string notiOut = "Out";

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Initialize(string title, string destination)
    {
        titleText.text = title;
        destinationText.text = destination;
        anim.Play(notiIn);
        Invoke(nameof(HideNotification), 3f);
    }

    public override void HideNotification()
    {
        CancelInvoke(nameof(HideNotification));
        if (gameObject.activeSelf)
            StartCoroutine(NotificationLifeCycle());
    }

    private IEnumerator NotificationLifeCycle()
    {
        anim.Play(notiOut);
        yield return new WaitForSeconds(2f);
        Despawn(this);
    }
    
    
    public static GeneralNotificationView Spawn(GameObject notificationPrefab, Transform notificationContainerTransform, string title, string destination)
    {
        var notificationInstance = Pool.Spawn(notificationPrefab, notificationContainerTransform);
        var notificationView = notificationInstance.GetComponent<GeneralNotificationView>();
        //Set the sibling index to the bot  of the stack
        notificationInstance.transform.SetAsLastSibling();
        notificationView.Initialize(title, destination);
        return notificationView;
    }
    
    public static void Despawn(NotificationView notification)
    {
        Pool.Despawn(notification.gameObject);
    }
}