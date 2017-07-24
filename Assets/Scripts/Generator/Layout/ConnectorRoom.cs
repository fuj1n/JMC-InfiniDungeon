using UnityEngine;

public class ConnectorRoom : DungeonRoom
{
    private bool longitudeBased;

    public ConnectorRoom(int width, int height, bool isLongitude) : base(width, height)
    {
        longitudeBased = isLongitude;
    }

    public override GameObject Generate(float x, float y, float z)
    {
        GameObject room = new GameObject("connector");
        Transform troom = room.transform;
        troom.position = new Vector3(x * TILE_WIDTH, y * TILE_HEIGHT, z * TILE_DEPTH);

        for (int tx = 0; tx < floors.GetLength(0); tx++)
            for (int tz = 0; tz < floors.GetLength(1); tz++)
                if (floors[tx, tz] != null)
                {
                    GameObject ob = Object.Instantiate(floors[tx, tz].gameObject, troom);
                    ob.transform.Translate(tx * TILE_WIDTH, y * TILE_HEIGHT, tz * TILE_DEPTH);
                    if (longitudeBased)
                        ob.transform.Rotate(0, 90, 0);
                }

        return room;
    }
}
