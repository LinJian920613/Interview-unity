using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Map : BaseUI
{
    public Button StartBtn;
    public Button TipBtn;
    public Button CloseBtn;
    public Button ResetBtn;
    public Button DoneBtn;
    public GameObject DoneGO;
    public GameObject StepGO;
    public Text Step;
    public GameObject ItemPFB;
    public Transform Panel;

    public PointEnterAndExit PointEnterAndExit;
    public GridLayoutGroup GridLayoutGroup;

    int current_step = 0;

    MapItem current_select;
    MapItem[] map_com_list;
    readonly List<MapItem> select_list = new List<MapItem>();
    readonly List<MapItem> canselect_list = new List<MapItem>();

    Question question;
    Coroutine coroutine;

    int playCnt = 0;
    bool inDoneState = false;

    int rows;
    int columns;


    protected override void OnEnter(object param)
    {
        SetStep(0);

        // 随机找题目
        List<Question> question_list = DataCenter.MapData.GetMapItems();
        question = question_list[UnityEngine.Random.Range(0, question_list.Count)];

        // 随机从行列列表中取数值
        var t = question.Tables[UnityEngine.Random.Range(0, question.Tables.Count)];
        rows = t.Row;
        columns = t.Column;

        GridLayoutGroup.constraintCount = t.Column;

        map_com_list = new MapItem[rows * columns];

        for (int i = 0; i < question.item_list.Count; i++)
        {
            var item = question.item_list[i];
            var item_go = GameObject.Instantiate(ItemPFB);
            item_go.SetActive(true);
            item_go.SetParent(Panel, true);
            MapItem map_com = item_go.GetComponent<MapItem>();
            map_com.OnClick += OnMapItemClick;
            string sprite_name = item.Name;
            map_com.Set(i, Constant.MapAtlasName, sprite_name);

            int index = (item.Index.Row - 1) * columns + (item.Index.Column - 1);
            map_com_list[index] = map_com;
        }

        PointEnterAndExit.OnEnter += OnPointEnterStepArea;
        PointEnterAndExit.OnExit += OnPointExitStepArea;
        ResetBtn.onClick.AddListener(DoResetStart);
        DoneBtn.onClick.AddListener(OnDoneBtnClick);
    }

    protected override void OnExit(object param)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        playCnt = 0;
        PointEnterAndExit.OnEnter -= OnPointEnterStepArea;
        PointEnterAndExit.OnExit -= OnPointExitStepArea;
        ResetBtn.onClick.RemoveListener(DoResetStart);
        DoneBtn.onClick.RemoveListener(OnDoneBtnClick);
        question = default;
        SetStep(0);
    }

    void DoReset()
    {
        current_select = null;
        for (int i = 0; i < select_list.Count; i++)
        {
            select_list[i]?.ResetState();
        }
        select_list.Clear();
        for (int i = 0; i < canselect_list.Count; i++)
        {
            canselect_list[i]?.ResetState();
        }
        canselect_list.Clear();
        SetStep(0);
        DoneGO.SetActive(false);
        StepGO.SetActive(true);
    }

    public void DoResetStart()
    {
        DoReset();
        playCnt = 0;
    }

    void OnDoneBtnClick()
    {
        if (inDoneState) return;
        inDoneState = true;
        if (playCnt >= DataCenter.MapData.MaxPlayCnt) return;
        
        ++playCnt;
        bool suc = IsRightAnswer();
        if (suc)
        {
            for (int i = 0; i < select_list.Count; i++)
            {
                select_list[i]?.SetLabel(MapItem.Result.Right);
            }
            inDoneState = false;
        }
        else 
        {
            coroutine = StartCoroutine(DoneLogic());
        }
    }

    IEnumerator DoneLogic()
    {
        var first = select_list[0];
        first.ScalUp();
        yield return new WaitForSeconds(1.5F);
        first.Shake();
        yield return new WaitForSeconds(1F);
        if (playCnt == DataCenter.MapData.MaxPlayCnt)
        {
            for (int i = 0; i < select_list.Count; i++)
            {
                select_list[i]?.SetLabel(MapItem.Result.Wrong);
            }
        }

        yield return new WaitForSeconds(1F);
        DoReset();
        yield return new WaitForSeconds(1.5F);
        if (playCnt == DataCenter.MapData.MaxPlayCnt)
        {
            List<int> r = question.Result[UnityEngine.Random.Range(0, question.Result.Count - 1)];
            for (int i = 0; i < r.Count; i++)
            {
                int target = r[i];
                var cur = map_com_list[target];
                cur.SetState(MapItem.State.Select, false);
                select_list.Add(cur);

                if (i >= 1)
                {
                    int pre_target = r[i - 1];
                    var prev = map_com_list[pre_target];
                    var dir = GetDir(prev.Index, cur.Index);
                    prev.SetArrow(dir);
                }
            }
        }
        inDoneState = false;
        StopCoroutine(coroutine);
        coroutine = null;
    }

    bool IsRightAnswer() 
    {
        for (int i = 0; i < question.Result.Count; i++)
        {
            var awser = question.Result[i];
            for (int j = 0; j < select_list.Count; j++)
            {
                bool ret = select_list.Compareto(awser);
                if (ret) return true;
            }
        }
        return false;
    }

    void OnPointEnterStepArea() 
    {
        if (current_step < DataCenter.MapData.TotalStep) return;
        DoneGO.SetActive(true);
        StepGO.SetActive(false);
    }

    void OnPointExitStepArea()
    {
        if (current_step < DataCenter.MapData.TotalStep) return;
        DoneGO.SetActive(false);
        StepGO.SetActive(true);
    }

    void SetStep(int step_id)
    {
        current_step = step_id;
        Step.text = string.Format(Constant.STEP_FORMAT, current_step, DataCenter.MapData.TotalStep);
    }

    void OnMapItemClick(MapItem map_item)
    {
        if (playCnt >= DataCenter.MapData.MaxPlayCnt) return;

        if (map_item.Status == MapItem.State.Select)
        {
            if (current_select != map_item)
            {
                ResetCanselect();
                for (int i = select_list.Count - 1; i >= 0; i--)
                {
                    MapItem item = select_list[i];
                    if (item == map_item)
                    {
                        current_select = map_item;
                        SetItemAroundCanSelect(map_item);
                        break;
                    }
                    else
                    {
                        item.SetState(MapItem.State.Default);
                        select_list.Remove(item);
                    }
                }

                SetStep(select_list.Count);
            }
        }
        else
        {
            if (current_step >= DataCenter.MapData.TotalStep) return;
            if (current_select != null && select_list.Count == 1 && map_item.Status == MapItem.State.Default)
            {
                ResetCanselect();
                current_select.SetState(MapItem.State.Default);
                select_list.Remove(current_select);

                map_item.SetState(MapItem.State.Select);
                current_select = map_item;
                SetItemAroundCanSelect(map_item);
                select_list.Add(map_item);
            }
            else
            {
                if (current_step == 0 || map_item.Status == MapItem.State.CanSelect)
                {
                    map_item.SetState(MapItem.State.Select);
                    if (current_select != null)
                    {
                        var dir = GetDir(current_select.Index, map_item.Index);
                        current_select.SetArrow(dir);
                    }
                    select_list.Add(map_item);
                    SetStep(++current_step);
                    current_select = map_item;

                    ResetCanselect();
                    if (current_step == DataCenter.MapData.TotalStep)
                    {
                        Debug.Log("步数已用完!");
                    }
                    else SetItemAroundCanSelect(map_item);
                }
            }
        }
    }

    void ResetCanselect()
    {
        for (int i = 0; i < canselect_list.Count; i++)
        {
            MapItem item = canselect_list[i];
            if (item.Status == MapItem.State.CanSelect) {
                item.SetState(MapItem.State.Default);
            }
        }
        canselect_list.Clear();
    }

    void SetItemAroundCanSelect(MapItem map_item)
    {
        int map_id = map_item.Index;
        List<int> target_id = new List<int>(4);

        int up = map_id - columns;
        if (InRange(up)) target_id.Add(up);
        int down = map_id + columns;
        if (InRange(down)) target_id.Add(down);
        int left = map_id - 1;
        if (InRange(left) && GameTools.IsSameRaw(left, map_id, columns)) target_id.Add(left);
        int right = map_id + 1;
        if (InRange(right) && GameTools.IsSameRaw(right, map_id, columns)) target_id.Add(right);

        for (int i = 0; i < target_id.Count; i++)
        {
            int id = target_id[i];
            var com = map_com_list[id];
            if (com.Status == MapItem.State.Default)
            {
                com.SetState(MapItem.State.CanSelect);
                canselect_list.Add(com);
            }
        }
    }

    bool InRange(int i) 
    {
        int max = rows * columns;
        return i >= 0 && i < max;
    }

    MapItem.ArrowDir GetDir(int prev, int now) 
    {
        var v = now - prev;
        if (v == 1) return MapItem.ArrowDir.Left;
        else if (v == -1) return MapItem.ArrowDir.Right;
        else if (v == columns) return MapItem.ArrowDir.Down;
        else if (v == -columns) return MapItem.ArrowDir.Up;
        throw new Exception("Unexpected value.");
    }
}
