using UnityEngine;

public struct IntCoords2
{
    public readonly int x, y;

    public IntCoords2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static implicit operator Vector3(IntCoords2 me)
    {
        return new Vector3(me.x, me.y);
    }

    public static explicit operator Vector2(IntCoords2 me)
    {
        return new Vector2(me.x, me.y);
    }
}

