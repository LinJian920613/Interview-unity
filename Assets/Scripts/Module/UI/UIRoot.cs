using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIRoot : MonoBehaviour
{
    public Camera UICamera;
    public Transform ContentTRS;
    public EventSystem EventSystem;

    readonly List<string> blockList = new List<string>();

    const int DEFALT_WAIT_TIME = 10000;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void AddChild(GameObject ui_go) 
    {
        var ui_trs = ui_go.transform;
        ui_trs.SetParent(ContentTRS);
        ui_trs.ResetLocal();
        ui_trs.SetAsLastSibling();
    }

    public void ShowWaitingBlock(string block_key, int wait_time = DEFALT_WAIT_TIME) 
    {
        blockList.Add(block_key);
    }

    public void HideWaitingBlock(string block_key)
    {
        blockList.Add(block_key);
    }
}