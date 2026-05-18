using System;
using System.Collections.Generic;
using System.Linq;

public static class IListExtensions
{
    public static void Shuffle<T>(this IList<T> ts)
    {
        int count = ts.Count;
        int last = count - 1;
        for (var i = 0; i < last; ++i) 
        {
            int r = UnityEngine.Random.Range(i, count);
            T tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static T Pop<T>(this IList<T> ts)
    {
        T last = ts[ts.Count - 1];

        ts.RemoveAt(ts.Count - 1);

        return last;
    }

    public static bool TryPop<T>(this IList<T> ts, out T last)
    {
        if (ts.Count > 0)
        {
            last = Pop(ts);
            return true;
        }
        else
        {
            last = default;
            return false;
        }
    }
}