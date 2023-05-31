using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class UITypes
{
    internal static UIType UIRoot = new UIType("UIRoot");
    internal static UIType Map = new UIType("Map");
}

public class UIManager : GameModule
{
    readonly Dictionary<int, UIGO> m_uiGoDic = new Dictionary<int, UIGO>();
    UIRoot m_uiRoot;

    public override void Init()
    {
        UIGO root_go = CreateUI(UITypes.UIRoot, true);
        if(root_go.IsValid) m_uiRoot = root_go.Go.GetComponent<UIRoot>();

        base.Init();
    }

    public override void Dispose()
    {
        foreach (var item in m_uiGoDic)
        {
            UIGO value = item.Value;
            if (value.Go) GameObject.Destroy(value.Go);
        }
        m_uiGoDic.Clear();
        base.Dispose();
    }

    int index;
    UIGO CreateUI(UIType uiType, bool isRoot = false)
    {
        GameObject pfb = Game.Asset.Load<GameObject>($"UI/{uiType.Path}");
        GameObject go = GameObject.Instantiate(pfb);
        if (isRoot) go.GetComponent<Transform>().ResetLocal();
        else m_uiRoot.AddChild(go);
        index += 1;
        UIGO uigo = UIGO.Acquire(index, uiType, go, isRoot);
        m_uiGoDic.Add(index, uigo);
        return uigo;
    }

    void DestroyUI(UIGO uigo)
    {
        if (uigo.Index == 0) return;
        GameObject.Destroy(uigo.Go);
        m_uiGoDic.Remove(uigo.Index);
        UIGO.Recycle(uigo);
    }

    public UIGO Open(UIType type, object param = null) 
    {
        UIGO ui = CreateUI(type);
        if (ui.IsValid) {
            ui.BaseUI.Enter(param);
            ui.BaseUI.Show();
        }
        return ui;
    }

    public void Close(int id, object param = null) 
    {
        if (!m_uiGoDic.ContainsKey(id)) return;
        UIGO uigo = m_uiGoDic[id];
        uigo.BaseUI.Hide();
        uigo.BaseUI.Exit(param);
        DestroyUI(uigo);
    }

    #region SpriteAtlas
    Dictionary<string, SpriteAtlas> atlasDic = new Dictionary<string, SpriteAtlas>();
    public Sprite GetSprite(string atlas_name, string sprite_name)
    {
        SpriteAtlas atlas;
        if (atlasDic.ContainsKey(atlas_name) == false)
        {
            atlas = Game.Asset.Load<SpriteAtlas>($"res/SpriteAtlas/{atlas_name}");
            atlasDic[atlas_name] = atlas;
        }
        else atlas = atlasDic[atlas_name];

        Sprite sprite = atlas.GetSprite(sprite_name);
        return sprite;
    }
    #endregion
}

public struct UIGO
{
    static readonly Queue<UIGO> s_cache = new Queue<UIGO>();

    public static UIGO Acquire(int id, UIType type, GameObject go, bool root) 
    {
        if (s_cache.Count > 0) {
            var uigo = s_cache.Dequeue();
            uigo.Set(id, type, go, root);
        }
        return new UIGO(id, type, go, root);
    }

    public static void Recycle(UIGO uigo) 
    {
        uigo.Reset();
        if (s_cache.Contains(uigo)) return;
        s_cache.Enqueue(uigo);
    }

    public bool IsValid;
    public int Index;
    public UIType Type;
    public GameObject Go;
    public BaseUI BaseUI;
    public bool Root;

    public UIGO(int id, UIType type, GameObject go, bool root)
    {
        Index = id;
        Type = type;
        Go = go;
        BaseUI = null;
        if (!root)
        {
            BaseUI = go.GetComponent<BaseUI>();
            BaseUI.Set(id, type);
        }
        Root = root;
        IsValid = Index > 0 && go != null && (Root || BaseUI != null);
    }

    public void Set(int id, UIType type, GameObject go, bool root)
    {
        Index = id;
        Type = type;
        Go = go;
        BaseUI = null;
        if (!root)
        {
            BaseUI = go.GetComponent<BaseUI>();
            BaseUI.Set(id, type);
        }
        Root = root;
        IsValid = Index > 0 && go != null && (Root || BaseUI != null);
    }

    public void Reset()
    {
        Index = 0;
        Go = null;
        IsValid = false;
        Root = false;
        BaseUI = null;
    }
}
