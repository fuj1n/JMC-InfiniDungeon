using UnityEngine;

public struct IntCoords3
{
    public readonly int x, y, z;

    public IntCoords3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public IntCoords3(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.z = 0;
    }

    public static implicit operator Vector3(IntCoords3 me)
    {
        return new Vector3(me.x, me.y, me.z);
    }

    public static explicit operator Vector2(IntCoords3 me)
    {
        return new Vector2(me.x, me.y);
    }
}
