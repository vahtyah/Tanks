using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerDownHandler : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent OnClick = new();
    
    public void OnPointerDown(PointerEventData eventData)
    {
        OnClick.Invoke();
    }
}
