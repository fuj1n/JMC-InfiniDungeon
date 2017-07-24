using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : System.Object
{
    public const int TILE_WIDTH = 5;
    public const int TILE_DEPTH = 5;
    public const int TILE_HEIGHT = 100;

    public readonly int width, height;

    public DungeonTile[,] floors;
    public DungeonTile[,] ceilings;
    public DungeonTile[,] walls;

    public Dictionary<CardinalDirection, Placement<DungeonTile>> exits = new Dictionary<CardinalDirection, Placement<DungeonTile>>();

    public List<Placement<DungeonZoner>> zoners = new List<Placement<DungeonZoner>>();
    public Dictionary<Placement<DungeonZoner>, Placement<DungeonZoner>> zonerLinks = new Dictionary<Placement<DungeonZoner>, Placement<DungeonZoner>>();

    public List<Placement<DungeonComponent>> components = new List<Placement<DungeonComponent>>();
    public List<Placement<EntityEnemy>> enemies = new List<Placement<EntityEnemy>>();

    public IntCoords3? position = null;
    public List<DungeonRoom> connections = new List<DungeonRoom>();

    public DungeonRoom(int width, int height)
    {
        this.width = width;
        this.height = height;

        floors = new DungeonTile[width, height];
        ceilings = new DungeonTile[width, height];
        walls = new DungeonTile[width, height];
    }

    public virtual GameObject Generate(float x, float y, float z)
    {
        GameObject room = new GameObject("room");
        Transform troom = room.transform;
        troom.position = new Vector3(x * TILE_WIDTH, y * TILE_HEIGHT, z * TILE_DEPTH);

        Transform floors = new GameObject("floors").transform;
        floors.SetParent(troom);
        floors.localPosition = Vector3.zero;
        for (int tx = 0; tx < this.floors.GetLength(0); tx++)
            for (int tz = 0; tz < this.floors.GetLength(1); tz++)
                if (this.floors[tx, tz] != null)
                {
                    GameObject ob = Object.Instantiate(this.floors[tx, tz].gameObject, floors);
                    ob.transform.Translate(tx * TILE_WIDTH, y * TILE_HEIGHT, tz * TILE_DEPTH);
                }
        Transform ceilings = new GameObject("ceilings").transform;
        ceilings.SetParent(troom);
        ceilings.localPosition = Vector3.zero;
        for (int tx = 0; tx < this.ceilings.GetLength(0); tx++)
            for (int tz = 0; tz < this.ceilings.GetLength(1); tz++)
                if (this.ceilings[tx, tz] != null)
                {
                    GameObject ob = Object.Instantiate(this.ceilings[tx, tz].gameObject, ceilings);
                    ob.transform.Translate(tx * TILE_WIDTH, y * TILE_HEIGHT, tz * TILE_DEPTH);
                }

        Transform walls = new GameObject("walls").transform;
        walls.SetParent(troom);
        walls.localPosition = Vector3.zero;
        for (int tx = 0; tx < this.walls.GetLength(0); tx++)
            for (int tz = 0; tz < this.walls.GetLength(1); tz++)
                if (this.walls[tx, tz] != null)
                {
                    GameObject go = Object.Instantiate(this.walls[tx, tz].gameObject, walls);
                    go.transform.Translate(tx * TILE_WIDTH, y * TILE_HEIGHT, tz * TILE_DEPTH);

                    if ((tx == 0 || tx == this.walls.GetLength(0) - 1) && (tz == 0 || tz == this.walls.GetLength(1) - 1))
                    {
                        GameObject go1 = Object.Instantiate(this.walls[tx, tz].gameObject, walls);
                        go1.transform.Translate(tx * TILE_WIDTH, y * TILE_HEIGHT, tz * TILE_DEPTH);
                        go.transform.Rotate(new Vector3(0, tz == 0 ? -90 : 90, 0));
                        go1.transform.Rotate(new Vector3(0, tx == 0 ? 0 : 180, 0));
                    }
                    else
                    {
                        if (tz == 0)
                            go.transform.Rotate(new Vector3(0, -90, 0));
                        else if (tz == this.walls.GetLength(1) - 1)
                            go.transform.Rotate(new Vector3(0, 90, 0));
                        else if (tx == this.walls.GetLength(0) - 1)
                            go.transform.Rotate(new Vector3(0, 180, 0));
                    }
                }

        Transform components = new GameObject("components").transform;
        components.SetParent(troom);
        components.localPosition = Vector3.zero;
        foreach (var component in this.components)
            Object.Instantiate(component.component.gameObject, new Vector3((component.x + x) * TILE_WIDTH, y * TILE_HEIGHT, (component.y + y) * TILE_DEPTH), Quaternion.identity, components);

        GameObject lgo = new GameObject("Light");
        lgo.transform.SetParent(troom);

        lgo.transform.localPosition = new Vector3(width / 2 * TILE_WIDTH, y * TILE_HEIGHT + 1F, height / 2 * TILE_DEPTH);

        Light l = lgo.AddComponent<Light>();
        l.type = LightType.Point;

        // TODO zoners and enemies

        return room;
    }

    public IntCoords2 ToRoomPosition(int x, int z)
    {
        return new IntCoords2(x - position.Value.x, z - position.Value.z);
    }

    public struct Placement<T>
    {
        public readonly DungeonRoom parent;

        public readonly float x, y;
        public readonly T component;

        public Placement(DungeonRoom parent, float x, float y, T component)
        {
            this.parent = parent;

            this.x = x;
            this.y = y;
            this.component = component;
        }
    }
}
