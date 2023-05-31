
public partial class Game
{
    public static AssetManager Asset;
    public static EventManager Event;
    public static UIManager UI;
    public static GameLancher Lancher;
    public static AudioManager Audio;
    public static DataTableManager Data;

    void OnInit()
    {
        Asset = Get<AssetManager>();
        Data = Get<DataTableManager>();
        Event = Get<EventManager>();
        Audio = Get<AudioManager>();
        UI = Get<UIManager>();
        Lancher = Get<GameLancher>();
    }
}