using System;
using System.Collections.Generic;
using UnityEngine;

public class ModuleFacade
{
    readonly List<GameModule> m_modulelist = new List<GameModule>();
    public bool InitFinish;
    public Action OnInitFinish;

    public void Init()
    {
        Add<AssetManager>();
        Add<DataTableManager>();
        Add<EventManager>();
        Add<AudioManager>();
        Add<UIManager>();
        Add<GameLancher>();
    }

    public void Run()
    {
        int len = m_modulelist.Count;
        if (len == 0) return;

        GameModule end_module = m_modulelist[len - 1];
        end_module.OnInitFinish += () => {
            InitFinish = true;
            OnInitFinish?.Invoke();
        };

        for (int i = 0; i < len - 1; i++)
        {
            GameModule module = m_modulelist[i];
            int next_index = i + 1;
            GameModule next_model = m_modulelist[next_index];
            module.OnInitFinish += next_model.Init;
        }

        GameModule first_module = m_modulelist[0];
        first_module.Init();
    }

    public void LateUpdate(float deltaTime)
    {
        for (int i = 0; i < m_modulelist.Count; i++)
        {
            var module = m_modulelist[i];
            if (module == null || !module.Enable) continue;
            module.LateUpdate(deltaTime);
        }
    }

    public void Update(float deltaTime)
    {
        for (int i = 0; i < m_modulelist.Count; i++)
        {
            var module = m_modulelist[i];
            if (module == null || !module.Enable) continue;
            module.Update(deltaTime);
        }
    }

    public void FixedUpdate(float fixedDeltaTime)
    {
        for (int i = 0; i < m_modulelist.Count; i++)
        {
            var module = m_modulelist[i];
            if (module == null || !module.Enable) continue;
            module.FixedUpdate(fixedDeltaTime);
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < m_modulelist.Count; i++)
        {
            var module = m_modulelist[i];
            if (module == null) continue;
            module.Dispose();
        }
        m_modulelist.Clear();
        OnInitFinish = null;
        InitFinish = false;
    }

    public T Add<T>() where T : GameModule, new()
    {
        T module = new T();
        int target_index = -1;
        for (int i = 0; i < m_modulelist.Count; i++)
        {
            var _module = m_modulelist[i];
            if (_module.Priority > module.Priority)
            {
                target_index = i;
                break;
            }
        }
        target_index = target_index == -1 ? m_modulelist.Count : target_index;
        m_modulelist.Insert(target_index, module);
        return module;
    }

    public void Remove(GameModule module)
    {
        m_modulelist.Remove(module);
    }

    public T Get<T>() where T : GameModule
    {
        for (int i = 0; i < m_modulelist.Count; i++)
        {
            var _module = m_modulelist[i];
            if (_module is T)
            {
               return _module as T;
            }
        }
        return null;
    }
}

public abstract class GameModule
{
    bool _enable = true;
    public bool Enable
    {
        get { return _enable; }
        set
        {
            _enable = value;
            if (value) OnEnable();
            else OnDisable();
        }
    }

    public bool BlInit { get; protected set; }
    public int Priority { get; set; }
    public Action OnInitFinish { get; set; }
    public Action OnDispose { get; set; }

    public virtual void Init()
    {
        BlInit = true;
        OnInitFinish?.Invoke();
    }

    public virtual void LateUpdate(float deltaTime) { }

    public virtual void Update(float deltaTime) { }

    public virtual void FixedUpdate(float fixedDeltaTime) { }

    public virtual void Dispose()
    {
        OnDispose?.Invoke();
        OnDispose = null;
        Enable = true;
        OnInitFinish = null;
    }

    protected virtual void OnEnable() { }

    protected virtual void OnDisable() { }
}