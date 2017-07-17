using System.Collections.Generic;
using System.Linq;

public static class Extensions {
    public static List<TSource> RemoveInst<TSource>(this List<TSource> lst, TSource rem)
    {
        List<TSource> cpy = new List<TSource>(lst);

        cpy.Remove(rem);

        return cpy;
    }
}
