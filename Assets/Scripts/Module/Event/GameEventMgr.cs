using System;
using System.Collections.Generic;

public class GameEvent
{
}

public class GameEventMgr
{

    static GameEventMgr _instance = null;
    public static GameEventMgr Instance
    {
        get
        {
            return _instance ?? (_instance = new GameEventMgr());
        }
    }

    static bool _isSafeMode = false;

    // Explicitly initialize GameEventMgr, and enable/disable debug mode
    public static void Init(bool is_debug_enable = false)
    {
        _isSafeMode = is_debug_enable;

        if (_instance == null)
        {
            _instance = new GameEventMgr();
        }
    }

    public delegate void EventHandler<T>(T e) where T : GameEvent;
    public delegate void EventHandler(GameEvent e);
    public delegate void NoArgsEventHandler();

    private Dictionary<Type, EventHandler> _delegates = new Dictionary<Type, EventHandler>();
    private Dictionary<Delegate, EventHandler> _delegateLookup = new Dictionary<Delegate, EventHandler>();
    private Dictionary<int, NoArgsEventHandler> _noArgsDelegates = new Dictionary<int, NoArgsEventHandler>();

    private GameEventMgr() { }

    public void AddListener<T>(EventHandler<T> handler) where T : GameEvent
    {
        // Early-out if we've already registered this delegate
        if (_delegateLookup.ContainsKey(handler))
        {
            return;
        }

        // Create a new non-generic delegate which calls our generic one.
        // This is the delegate we actually invoke.
        void internalDelegate(GameEvent e) => handler((T)e);
        _delegateLookup[handler] = internalDelegate;

        if (_delegates.TryGetValue(typeof(T), out EventHandler handler_out))
        {
            _delegates[typeof(T)] = handler_out += internalDelegate;
        }
        else
        {
            _delegates[typeof(T)] = internalDelegate;
        }
    }

    public void AddListener(int event_id, NoArgsEventHandler handler)
    {
        if (_noArgsDelegates.TryGetValue(event_id, out NoArgsEventHandler handler_out))
        {
            _noArgsDelegates[event_id] = handler_out += handler;
        }
        else
        {
            _noArgsDelegates[event_id] = handler;
        }
    }

    public void AddListener(string typeName, EventHandler handler)
    {
        var type = Type.GetType(typeName);
        if (_delegateLookup.ContainsKey(handler))
        {
            return;
        }

        void internalDelegate(GameEvent e) => handler(e);
        _delegateLookup[handler] = internalDelegate;

        if (_delegates.TryGetValue(type, out EventHandler handler_out))
        {
            _delegates[type] = handler_out += internalDelegate;
        }
        else
        {
            _delegates[type] = internalDelegate;
        }
    }

    public void RemoveListener<T>(EventHandler<T> handler) where T : GameEvent
    {
        if (_delegateLookup.TryGetValue(handler, out EventHandler internal_handler))
        {
            if (_delegates.TryGetValue(typeof(T), out EventHandler handler_out))
            {
                handler_out -= internal_handler;

                if (handler_out == null)
                {
                    _delegates.Remove(typeof(T));
                }
                else
                {
                    _delegates[typeof(T)] = handler_out;
                }
            }

            _delegateLookup.Remove(handler);
        }
    }

    public void RemoveListener(int event_id, NoArgsEventHandler handler)
    {
        if (_noArgsDelegates.TryGetValue(event_id, out NoArgsEventHandler handler_out))
        {
            handler_out -= handler;

            if (handler_out == null)
            {
                _noArgsDelegates.Remove(event_id);
            }
            else
            {
                _noArgsDelegates[event_id] = handler_out;
            }
        }
    }

    public void RemoveListener(string typeName, EventHandler handler)
    {
        var type = Type.GetType(typeName);

        if (_delegateLookup.TryGetValue(handler, out EventHandler internal_handler))
        {
            if (_delegates.TryGetValue(type, out EventHandler handler_out))
            {
                handler_out -= internal_handler;
                if (handler_out == null)
                {
                    _delegates.Remove(type);
                }
                else
                {
                    _delegates[type] = handler_out;
                }
            }

            _delegateLookup.Remove(handler);
        }
    }

    public void Raise(GameEvent e)
    {
        // Early out if no one is listening.
        if (!_delegates.TryGetValue(e.GetType(), out EventHandler handler))
        {
            return;
        }

        if (_isSafeMode)
        {
            SafeInvokeHandler(handler, e);
        }
        else
        {
            handler.Invoke(e);
        }
    }

    public void Raise(int event_id)
    {
        // Early out if no one is listening.
        if (!_noArgsDelegates.TryGetValue(event_id, out NoArgsEventHandler handler))
        {
            return;
        }

        if (_isSafeMode)
        {
            SafeInvokeNoArgsHandler(handler, event_id);
        }
        else
        {
            handler.Invoke();
        }
    }

    public void Clear()
    {
        _delegates.Clear();
        _delegateLookup.Clear();
        _noArgsDelegates.Clear();
    }

    // More secure way to invoke method instead of using multicast.
    // This way is more expensive, but is more secure to make sure one single
    // bad input cannot bring the whole system down.
    void SafeInvokeHandler(EventHandler handler, params GameEvent[] events)
    {
        Delegate[] delegate_array = handler.GetInvocationList();
        int count = delegate_array.Length;

        for (int i = 0; i < count; i++)
        {
            var del_item = delegate_array[i];

            try
            {
                if (del_item != null)
                {
                    //del_item.DynamicInvoke(e);
                    del_item.Method.Invoke(del_item.Target, events);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                string stack_trace = ex.StackTrace;

                if (ex.InnerException != null)
                {
                    msg = ex.InnerException.Message;
                    stack_trace = ex.InnerException.StackTrace;
                }

                UnityEngine.Debug.LogErrorFormat("[GameEventMgr]Raise: {0}\nStack Trace: {1}\nThis event listener will be removed because of this exception!", msg, stack_trace);

                // Clean up this handler
                var evt_type = events[0].GetType();
                var item_evt_handler = del_item as EventHandler;

                if (item_evt_handler != null)
                {
                    handler -= item_evt_handler;
                }

                if (handler == null)
                {
                    _delegates.Remove(evt_type);
                }
                else
                {
                    _delegates[evt_type] = handler;
                }

                var etor = _delegateLookup.GetEnumerator();

                while (etor.MoveNext())
                {
                    if (etor.Current.Value == item_evt_handler)
                    {
                        _delegateLookup.Remove(etor.Current.Key);
                        break;
                    }
                }
            }
        }
    }

    // More secure way to invoke method instead of using multicast.
    // This way is more expensive, but is more secure to make sure one single
    // bad input cannot bring the whole system down.
    void SafeInvokeNoArgsHandler(NoArgsEventHandler handler, int event_id)
    {
        Delegate[] delegate_array = handler.GetInvocationList();
        int count = delegate_array.Length;

        for (int i = 0; i < count; i++)
        {
            var del_item = delegate_array[i];

            try
            {
                if (del_item != null)
                {
                    //del_item.DynamicInvoke(e);
                    del_item.Method.Invoke(del_item.Target, null);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                string stack_trace = ex.StackTrace;

                if (ex.InnerException != null)
                {
                    msg = ex.InnerException.Message;
                    stack_trace = ex.InnerException.StackTrace;
                }

                UnityEngine.Debug.LogErrorFormat("[GameEventMgr]Raise: {0}\nStack Trace: {1}\nThis event listener will be removed because of this exception!", msg, stack_trace);

                // Clean up this handler
                var item_evt_handler = del_item as NoArgsEventHandler;

                if (item_evt_handler != null)
                {
                    handler -= item_evt_handler;
                }

                if (handler == null)
                {
                    _noArgsDelegates.Remove(event_id);
                }
                else
                {
                    _noArgsDelegates[event_id] = handler;
                }
            }
        }
    }


}
