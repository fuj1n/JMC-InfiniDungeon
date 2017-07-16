using UnityEngine;

public abstract class DungeonComponent : MonoBehaviour
{
    protected System.Random random = new System.Random();

    public abstract void Place(DungeonRoom room);
}
