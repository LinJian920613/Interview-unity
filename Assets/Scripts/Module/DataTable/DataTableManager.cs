using System.Collections.Generic;

public class DataTableManager : GameModule
{
    public JSData JSData { get; private set; }

    readonly List<IData> m_list = new List<IData>();
    public override void Init() 
    {
        JSData = AddData<JSData>();
        base.Init();
    }

    public override void Dispose()
    {
        for (int i = 0; i < m_list.Count; i++)
        {
            m_list[i]?.Dispose();
        }
        m_list.Clear();
        base.Dispose();
    }

    T AddData<T>() where T : IData, new()
    {
        T _data = new T();
        _data.Init();
        m_list.Add(_data);
        return _data;
    }
}

public interface IData 
{
    void Init();
    void Dispose();
}
