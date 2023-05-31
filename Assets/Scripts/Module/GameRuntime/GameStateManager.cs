using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameStateAttribute : Attribute
{
    public int State { get; set; }

    public GameStateAttribute(int state)
    {
        State = state;
    }
}

public abstract class GameState
{
    public virtual bool SupportMultiTouch { get; } = false;

    protected GameStateManager Manager;

    protected virtual void OnEnter(object param) { }
    public void Enter(GameStateManager manager, object param)
    {
        Manager = manager;
        OnEnter(param);
    }

    protected virtual void OnUpdate(float deltaTime) { }
    public void Update(float deltaTime) { OnUpdate(deltaTime); }

    protected virtual void OnLateUpdate(float deltaTime) { }
    public void LateUpdate(float deltaTime) { OnLateUpdate(deltaTime); }

    protected virtual void OnFixedUpdate(float fixedDeltaTime) { }
    public void FixedUpdate(float deltaTime) { OnFixedUpdate(deltaTime); }

    protected virtual void OnExit() { }
    public void Exit() { OnExit(); }

    protected T SetGameState<T>(object param = null) where T : GameState
    {
        return Manager.ChangeState<T>(param);
    }
}

public class GameStateManager
{
    public const int GAME_STATE_INVALID = 0;

    private GameState _state;
    public int State { get; private set; }
    public int PrevState { get; private set; }
    public int PendingState { get; private set; } = GAME_STATE_INVALID;

    GameState _newState = null;

    object _nextstateParam;

    /// <summary>
    /// Set next game state.
    /// It will mark the new state, and transition to the new state right after next update loop.
    /// </summary>
    /// <typeparam name="T">new game state</typeparam>
    /// <param name="transition">Action called before old state end</param>
    public T ChangeState<T>(object param = null) where T : GameState
    {
        GameStateAttribute attr = typeof(T).GetCustomAttributes(typeof(GameStateAttribute), false)[0] as GameStateAttribute;
        int newstate_int = attr.State;

        // Trying to set the same as current state, ignore it.
        if (newstate_int == State)
        {
            Debug.LogErrorFormat($"[GameStateMgr]SetGameState: Trying set the same state {typeof(T).Name}");
            return null;
        }
        else
        {
            T newstate_obj = (T)Activator.CreateInstance(typeof(T), true);
            _nextstateParam = param;
            PendingState = newstate_int;
            _newState = newstate_obj;
            Debug.LogFormat($"[GameStateMgr]SetGameState: {newstate_obj.GetType().Name}");
            return newstate_obj;
        }
    }

    void StartNewState()
    {
        if (_newState == null)
        {
            Debug.LogErrorFormat($"[GameStateMgr]StartNewState: New state object is null, fail to start new state(id:{PendingState})");
            PendingState = GAME_STATE_INVALID;
            return;
        }

        Debug.LogFormat($"[GameStateMgr]StartNewState: {_newState.GetType().Name}");

        // Deactivate input during game state transition
        if (EventSystem.current != null && EventSystem.current.currentInputModule != null)
        {
            EventSystem.current.currentInputModule.DeactivateModule();
        }

        GameState oldStateObj = _state;
        if (null != oldStateObj) oldStateObj.Exit();

        PrevState = State;
        State = PendingState;
        _state = _newState;
        PendingState = GAME_STATE_INVALID;
        _newState = null;

        // Reactivate input after game state transition finished
        if (EventSystem.current != null && EventSystem.current.currentInputModule != null)
        {
            EventSystem.current.currentInputModule.ActivateModule();
        }

        Input.multiTouchEnabled = _state.SupportMultiTouch;
        _state.Enter(this, _nextstateParam);
    }

    /// <summary>
    /// Call this to exit game
    /// </summary>
    public void ExitGame()
    {
        Debug.LogFormat($"[GameStateMgr]ExitGame");

        PlayerPrefs.Save();
        Application.runInBackground = false;
        Application.Quit();
    }

    public void Update(float deltaTime)
    {
        if (null != _state) _state.Update(deltaTime);
        if (PendingState != GAME_STATE_INVALID) StartNewState();
    }

    public void LateUpdate(float deltaTime)
    {
        if (null != _state) _state.LateUpdate(deltaTime);
    }

    public void FixedUpdate(float deltaTime)
    {
        if (null != _state) _state.FixedUpdate(deltaTime);
    }
}