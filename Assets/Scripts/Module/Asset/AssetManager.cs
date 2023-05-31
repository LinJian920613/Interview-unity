using System;
using UnityEngine;

public class AssetManager : GameModule
{
    public static Func<IRerouce> CreateHandler { get; set; } = ResourceHandler.CreateInstance;
    public IRerouce Handler { get; } = CreateHandler();

    public T Load<T>(string path) where T : UnityEngine.Object
    {
        return Handler.Load<T>(path);
    }
}

public interface IRerouce 
{
    T Load<T>(string path) where T : UnityEngine.Object;
}

public class ResourceHandler : IRerouce
{
    public static ResourceHandler CreateInstance() 
    {
        return new ResourceHandler();
    }

    public T Load<T>(string path) where T : UnityEngine.Object
    {
        return Resources.Load<T>(path);
    }
}