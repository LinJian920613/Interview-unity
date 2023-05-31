
public class EventManager : GameModule
{
    public override void Init()
    {
        Init(true);
        base.Init();
    }

    public override void Dispose()
    {
        Clear();
        base.Dispose();
    }

    // Explicitly initialize GameEventMgr, and enable/disable debug mode
    public void Init(bool is_debug_enable = false)
    {
        GameEventMgr.Init(is_debug_enable);
    }

    public void AddListener<T>(GameEventMgr.EventHandler<T> handler) where T : GameEvent
    {
        GameEventMgr.Instance.AddListener<T>(handler);
    }

    public void AddListener(int event_id, GameEventMgr.NoArgsEventHandler handler)
    {
        GameEventMgr.Instance.AddListener(event_id, handler);
    }

    public void AddListener(string typeName, GameEventMgr.EventHandler handler)
    {
        GameEventMgr.Instance.AddListener(typeName, handler);
    }

    public void RemoveListener<T>(GameEventMgr.EventHandler<T> handler) where T : GameEvent
    {
        GameEventMgr.Instance.RemoveListener<T>(handler);
    }

    public void RemoveListener(int event_id, GameEventMgr.NoArgsEventHandler handler)
    {
        GameEventMgr.Instance.RemoveListener(event_id, handler);
    }

    public void RemoveListener(string typeName, GameEventMgr.EventHandler handler)
    {
        GameEventMgr.Instance.RemoveListener(typeName, handler);
    }

    public void Raise(GameEvent e)
    {
        GameEventMgr.Instance.Raise(e);
    }

    public void Raise(int event_id)
    {
        GameEventMgr.Instance.Raise(event_id);
    }

    public void Clear()
    {
        GameEventMgr.Instance.Clear();
    }
}