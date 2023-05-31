
using LitJson;

public static class LitJsonUtils
{
    public static string ToString(this JsonData js, string key)
    {
        var data = js[key];
        if (data != null)
        {
            string s = (string)data;
            return s;
        }
        return null;
    }

    public static int ToInt(this JsonData js, string key)
    {
        var data = js[key];
        if (data != null)
        {
            int n = (int)data;
            return n;
        }
        return 0;
    }

    public static double ToDouble(this JsonData js, string key)
    {
        var data = js[key];
        if (data != null)
        {
            double f = (double)data;
            return f;
        }
        return 0;
    }

    public static bool ToBool(this JsonData js, string key)
    {
        var data = js[key];
        if (data != null)
        {
            bool b = (bool)data;
            return b;
        }
        return false;
    }

    public static object ToObject(this JsonData js, string key)
    {
        var data = js[key];
        if (data != null)
        {
            object o = (object)data;
            return o;
        }
        return null;
    }

    public static object ToObject(this JsonData js)
    {
        if (js != null)
        {
            object o = (object)js;
            return o;
        }
        return null;
    }
}