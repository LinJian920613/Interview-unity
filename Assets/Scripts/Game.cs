using UnityEngine;

public partial class Game : MonoBehaviour
{
    ModuleFacade Facade;
    static bool shuttingDown = false;

    public static bool IsShuttingDown
    {
        get { return shuttingDown; }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if(Facade == null) Facade = new ModuleFacade();
        Facade.Init();
        OnInit();

        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.lowMemory += OnLowMemory;
    }

    private void OnLowMemory()
    {
        Debug.LogWarning("[GameEngine]OnLowMemory: Memory is low!");
        Resources.UnloadUnusedAssets();
    }

    private void Start()
    {
        Facade.Run();
    }

    private void LateUpdate()
    {
        if (!Facade.InitFinish) return;
        Facade.LateUpdate(Time.deltaTime);
    }

    private void Update()
    {
        if (!Facade.InitFinish) return;
        Facade.Update(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!Facade.InitFinish) return;
        Facade.FixedUpdate(Time.fixedDeltaTime);
    }

    private void OnDestroy()
    {
        Application.lowMemory -= OnLowMemory;
        Facade.Dispose();
        Facade = null;
    }

    void OnApplicationQuit()
    {
        Debug.Log("[GameEngine]OnApplicationQuit");
        shuttingDown = true;
        PlayerPrefs.Save();
    }

    T Get<T>() where T : GameModule
    {
        return Facade.Get<T>();
    }
}