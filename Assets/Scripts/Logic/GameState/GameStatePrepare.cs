
[GameState((int)GameStates.Prepare)]
public class GameStatePrepare : GameState
{
    protected override void OnEnter(object param)
    {
        DataCenter.Init();
        SetGameState<GameStatePlay>();
    }
}