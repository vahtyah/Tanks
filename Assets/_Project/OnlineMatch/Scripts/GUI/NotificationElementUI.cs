using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationElementUI : MonoBehaviour
{
    public static NotificationElementUI Create(GameObject prefab, Transform container, string killerName,
        string victimName)
    {
        var notificationElement = Pool.Spawn(prefab, container).GetComponent<NotificationElementUI>();
        notificationElement.transform.SetSiblingIndex(0);
        notificationElement.Initialize(killerName, victimName);
        return notificationElement;
    }

    public static void Destroy(NotificationElementUI notificationElement)
    {
        Pool.Despawn(notificationElement.gameObject);
    }

    [SerializeField] private TextMeshProUGUI killerNameText;
    [SerializeField] private TextMeshProUGUI victimNameText;


    private Animator anim;
    // private string notiInLeft = "In";
    // private string notiOutRight = "Out";

    private string notiInRight = "InRight";
    private string notiOutLeft = "OutLeft";

    private IEnumerator iEnumerator;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Initialize(string killerName, string victimName)
    {
        killerNameText.text = killerName;
        victimNameText.text = victimName;
        anim.Play(notiInRight);

        Invoke(nameof(DisableNotification), 3f);
    }

    public void DisableNotification()
    {
        CancelInvoke(nameof(DisableNotification));
        if (gameObject.activeSelf)
            StartCoroutine(DestroyNotification());
    }

    IEnumerator DestroyNotification()
    {
        anim.Play(notiOutLeft);
        yield return new WaitForSeconds(2f);
        NotificationElementUI.Destroy(this);
    }
}