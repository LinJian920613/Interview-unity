
public class UIType
{
    public string Path { get; private set; }

    public string Name { get; private set; }

    public UIType(string path)
    {
        Path = path;
        Name = path.Substring(path.LastIndexOf('/') + 1);
    }

    public override string ToString()
    {
        return string.Format("UIType {0}({1})", Name, Path);
    }

    public override bool Equals(object obj)
    {
        if (obj is UIType otherType)
        {
            if (this.Path == otherType.Path)
            {
                return true;
            }
        }

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return Path.GetHashCode();
    }
}