#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsPressed { get; private set; }

    public UnityEvent OnPointerDownEvent;
    public UnityEvent OnPointerUpEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        OnPointerDownEvent.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        OnPointerUpEvent.Invoke();
    }
}
#endif