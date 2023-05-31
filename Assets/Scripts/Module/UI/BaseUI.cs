using System;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    public int UiIndex { get; private set; } = -1;
    public UIType UIType { get; private set; }

    public void Set(int id, UIType type)
    {
        UiIndex = id;
        UIType = type;
    }

    protected abstract void OnEnter(object param);
    public void Enter(object param) { OnEnter(param); }

    protected abstract void OnExit(object param);
    public void Exit(object param)
    {
        OnExit(param);
        UiIndex = -1;
    }

    protected virtual void OnShow() { }
    public void Show() { OnShow(); }

    protected virtual void OnHide() { }
    public void Hide() { OnHide(); }

}