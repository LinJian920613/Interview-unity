using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
[AddComponentMenu("UGUI/UI/UIButtonScale")]
public class ButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject target;
    public float fSpeed = 0.02f;
    public Vector3 normalV3 = Vector3.one;
    public Vector3 offsetV3 = new Vector3(0.8f, 0.8f, 0.8f);

    private bool __bScale = false; //false为缩小 ture为变大
    private bool __bRunning = false;

    private Button m_CurrentButton;

    public bool BlResetScaleOnEnable = true;
    public bool UseNormalScale = true;

    Vector3 _targetV3;

    void Start()
    {
        if (null != target)
        {
            m_CurrentButton = target.GetComponent<Button>();
        }
        else
        {
            m_CurrentButton = gameObject.GetComponent<Button>();
            if (m_CurrentButton) target = gameObject;
        }

        _targetV3 = UseNormalScale ? normalV3 : transform.localScale;
    }

    void OnEnable()
    {
        if(BlResetScaleOnEnable) SetNormal();
    }

    void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SetNormal();
        }
    }

    public void OnPointerDown(PointerEventData __eventData)
    {
        if (null == target)
            return;

        if (null != m_CurrentButton)
        {
            if (!m_CurrentButton.interactable)
                return;
        }

        __bScale = false;
        __bRunning = true;
    }

    public void OnPointerUp(PointerEventData __eventData)
    {
        if (null == target)
            return;

        if (null != m_CurrentButton)
        {
            if (!m_CurrentButton.interactable)
                return;
        }

        __bScale = true;
        __bRunning = true;
    }
    public void SetNormal()
    {
        __bScale = true;
        __bRunning = true;
    }

    void LateUpdate()
    {
        if (null == target)
            return;

        if (!__bRunning)
            return;

        if (!__bScale)
        {
            target.transform.localScale -= Vector3.one * fSpeed;
            if (target.transform.localScale.x <= offsetV3.x)
            {
                target.transform.localScale = offsetV3;
                __bRunning = false;
            }
        }
        else
        {
            target.transform.localScale += Vector3.one * fSpeed;
            if (target.transform.localScale.x >= _targetV3.x)
            {
                target.transform.localScale = _targetV3;
                __bRunning = false;
            }
        }
    }
}
