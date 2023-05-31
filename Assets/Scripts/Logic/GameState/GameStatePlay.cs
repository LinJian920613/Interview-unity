
[GameState((int)GameStates.GamePlay)]
public class GameStatePlay : GameState
{
    protected override void OnEnter(object param)
    {
        Game.UI.Open(UITypes.Map);
    }
}