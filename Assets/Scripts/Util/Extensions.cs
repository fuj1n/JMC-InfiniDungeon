using System;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static List<TSource> RemoveInst<TSource>(this List<TSource> lst, TSource rem)
    {
        List<TSource> cpy = new List<TSource>(lst);

        cpy.Remove(rem);

        return cpy;
    }

    public static bool NextBool(this System.Random random, int probability)
    {
        int chance = random.Next(1, 101);

        return chance <= probability;
    }

    public static T NextFrom<T>(this System.Random random, T[] array)
    {
        if (array.Length == 0)
            return default(T);
        return array[random.Next(0, array.Length)];
    }

    public static Bounds OrthographicBounds(this Camera camera)
    {
        float screenAspect = Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        return new Bounds(camera.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
    }
}
