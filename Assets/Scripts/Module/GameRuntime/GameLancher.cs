using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum GameStates
{
    None = 0,
    Prepare,
    Login,
    GamePlay,
}

public class GameLancher : GameModule
{
    readonly GameStateManager StateMgr = new GameStateManager();

    public override void Init()
    {
        ChangeState<GameStatePrepare>();
        base.Init();
    }

    public override void Dispose()
    {
        base.Dispose();
    }

    public override void Update(float deltaTime)
    {
        StateMgr.Update(deltaTime);
    }

    public override void LateUpdate(float deltaTime)
    {
        StateMgr.LateUpdate(deltaTime);
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
        StateMgr.FixedUpdate(fixedDeltaTime);
    }

    public T ChangeState<T>(object param = null) where T : GameState 
    {
        return StateMgr.ChangeState<T>(param);
    }
}