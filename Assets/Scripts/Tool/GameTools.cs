using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class GameTools
{
    public static void ResetLocal(this Transform trs) 
    {
        trs.localPosition = Vector3.zero;
        trs.localRotation = Quaternion.identity;
        trs.localScale = Vector3.one;
    }

    public static void SetParent(this GameObject go, Transform parent, bool resetlocal = false)
    {
        var TRS = go.GetComponent<Transform>();
        TRS.SetParent(parent);
        if(resetlocal) TRS.ResetLocal();
    }

    public static bool IsSameRaw(int i, int j, int columns)
    {
        int r1 = GetRaw(i, columns);
        int r2 = GetRaw(j, columns);
        return r1 == r2;
    }

    public static int GetRaw(int i, int columns)
    {
        int _t = i + 1;
        int _div = (int)(_t * 1.0F / columns);
        int _remainder = _t % columns;
        int r = _div + (_remainder > 0 ? 1 : 0);
        return r;
    }

    public static bool Compareto(this List<MapItem> map_items, List<int> l) 
    {
        if (map_items.Count != l.Count) return false;
        for (int i = 0; i < map_items.Count; i++)
        {
            if (map_items[i].Index != l[i]) return false;
        }
        return true;
    }
}