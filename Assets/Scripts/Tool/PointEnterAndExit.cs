using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointEnterAndExit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Action OnEnter;
    public Action OnExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit?.Invoke();
    }

    void OnDestroy() 
    {
        OnEnter = OnExit = null;
    }
}