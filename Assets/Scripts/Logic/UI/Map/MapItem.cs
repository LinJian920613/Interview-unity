using DentedPixel.LTEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapItem : MonoBehaviour
{
    public enum Result
    {
        Right,
        Wrong
    }

    public enum State 
    {
        Default,
        Select,
        CanSelect,
        Wrong
    }

    public enum ArrowDir
    {
        Up,
        Down,
        Left,
        Right
    }

    public LeanTweenVisual Lt_scale;
    public LeanTweenVisual Lt_Scaleup;

    public int Index { get; private set; }
    public GameObject RetRoot;
    public GameObject[] Rets;
    public GameObject ArrowRoot;
    public GameObject[] Arrows;
    public Button Btn;
    public Image BGImg;
    public Image showImg;

    public float alpha_unselect = 0.3F;
    public float alpha_select = 1;

    public float ShakeRange;
    public float ShakeSpeed = 1000;
    public float ShakeTime = 10;

    public State Status;

    public Action<MapItem> OnClick;

    RectTransform rectTRS;
    float timer;

    float defaulPosx;
    bool rightShake = true;

    private void Awake()
    {
        Btn.onClick.AddListener(OnBtnClick);
        rectTRS = gameObject.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (doShake)
        {
            if (ShakeTime > timer)
            {
                if (rightShake)
                {
                    float move = Time.deltaTime * ShakeSpeed;
                    if (move < ShakeRange)
                    {
                        float x = rectTRS.anchoredPosition.x + move;
                        rectTRS.anchoredPosition = new Vector2(x, rectTRS.anchoredPosition.y);
                        if (rectTRS.anchoredPosition.x > defaulPosx &&
                            Mathf.Abs(rectTRS.anchoredPosition.x - defaulPosx) > ShakeRange)
                        {
                            rightShake = false;
                        }
                    }
                }
                else
                {
                    float move = Time.deltaTime * ShakeSpeed;
                    if (move < ShakeRange)
                    {
                        float x = rectTRS.anchoredPosition.x - move;
                        rectTRS.anchoredPosition = new Vector2(x, rectTRS.anchoredPosition.y);
                        if (rectTRS.anchoredPosition.x < defaulPosx &&
                            Mathf.Abs(rectTRS.anchoredPosition.x - defaulPosx) > ShakeRange)
                        {
                            rectTRS.anchoredPosition = new Vector2(x, rectTRS.anchoredPosition.y);
                            rightShake = true;
                        }
                    }
                }
            }
            else
            {
                rectTRS.anchoredPosition = new Vector2(defaulPosx, rectTRS.anchoredPosition.y);
                timer = 0;
                rightShake = true;
                doShake = false;
            }
            timer += Time.deltaTime;
        }
    }

    private void OnDestroy()
    {
        defaulPosx = 0;
        rectTRS = null;
        OnClick = null;
        timer = 0;
        rightShake = true;
        doShake = true;
    }

    public void Set(int id, string atlas_name, string img_name) 
    {
        Sprite sprite = Game.UI.GetSprite(atlas_name, img_name);
        showImg.sprite = sprite;
        SetState(State.Default);
        Index = id;
    }

    public void SetState(State state, object param = null)
    {
        switch (state)
        {
            case State.Default:
                BGImg.color = new Color(BGImg.color.r, BGImg.color.g, BGImg.color.b, alpha_unselect);
                showImg.color = new Color(showImg.color.r, showImg.color.g, showImg.color.b, alpha_unselect);
                ResetArrow();
                ResetLabel();
                break;
            case State.Select:
                BGImg.color = new Color(BGImg.color.r, BGImg.color.g, BGImg.color.b, alpha_select);
                showImg.color = new Color(showImg.color.r, showImg.color.g, showImg.color.b, alpha_select);
                ArrowRoot.SetActive(true);

                bool blplay = true;
                if (param != null) blplay = (bool)param;
                if (blplay) Lt_scale?.play();
                break;
            case State.CanSelect:
                BGImg.color = new Color(BGImg.color.r, BGImg.color.g, BGImg.color.b, alpha_select);
                showImg.color = new Color(showImg.color.r, showImg.color.g, showImg.color.b, alpha_select);
                break;
            case State.Wrong:
                break;
            default:
                break;
        }

        Status = state;
    }

    public void ResetArrow()
    {
        for (int i = 0; i < Arrows.Length; i++)
        {
            GameObject arrow = Arrows[i];
            arrow.SetActive(false);
        }
        ArrowRoot.SetActive(false);
    }

    public void SetArrow(ArrowDir arrowDir)
    {
        ArrowRoot.SetActive(true);
        for (int i = 0; i < Arrows.Length; i++)
        {
            GameObject arrow = Arrows[i];
            arrow.SetActive(i == (int)arrowDir);
        }
    }

    public void ResetLabel()
    {
        for (int i = 0; i < Rets.Length; i++)
        {
            GameObject ret = Rets[i];
            ret.SetActive(false);
        }
        RetRoot.SetActive(false);
    }

    public void SetLabel(Result result)
    {
        RetRoot.SetActive(true);
        for (int i = 0; i < Rets.Length; i++)
        {
            GameObject ret = Rets[i];
            ret.SetActive(i == (int)result);
        }
    }

    bool doShake = false;
    public void Shake()
    {
        defaulPosx = rectTRS.anchoredPosition.x;
        doShake = true;
    }

    public void ScalUp() 
    {
        Lt_Scaleup?.play();
    }

    void OnBtnClick()
    {
        OnClick?.Invoke(this);
    }

    public void ResetState()
    {
        SetState(State.Default);
    }
}