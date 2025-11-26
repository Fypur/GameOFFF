using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OsuBall : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action OnPointerDownEvent;
    public event Action OnPointerUpEvent;
    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownEvent.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpEvent.Invoke();
    }
}
