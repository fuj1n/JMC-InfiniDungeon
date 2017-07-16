using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : System.Object
{
    public const int TILE_WIDTH = 5;
    public const int TILE_DEPTH = 5;
    public const int TILE_HEIGHT = 100;

    public readonly int rootx, rooty, rootz;
    public readonly int width, height;
    public bool isSpawn = false;

    public DungeonTile[,] floors;
    public DungeonTile[,] ceilings;
    public DungeonTile[,] walls;

    public Dictionary<CardinalDirection, Placement<DungeonTile>> exits = new Dictionary<CardinalDirection, Placement<DungeonTile>>();

    public List<Placement<DungeonZoner>> zoners = new List<Placement<DungeonZoner>>();
    public Dictionary<Placement<DungeonZoner>, Placement<DungeonZoner>> zonerLinks = new Dictionary<Placement<DungeonZoner>, Placement<DungeonZoner>>();

    public List<Placement<DungeonComponent>> components = new List<Placement<DungeonComponent>>();
    public List<Placement<EntityEnemy>> enemies = new List<Placement<EntityEnemy>>();

    public DungeonRoom(int x, int y, int z, int width, int height)
    {
        rootx = x;
        rooty = y;
        rootz = z;
        this.width = width;
        this.height = height;

        floors = new DungeonTile[width, height];
        ceilings = new DungeonTile[width, height];
        walls = new DungeonTile[width, height];
    }

    public bool IsRoot(int x, int y, int z)
    {
        return x == rootx && y == rooty && z == rootz;
    }

    public GameObject Generate(float x, float y, float z)
    {
        GameObject room = new GameObject("room");
        Transform troom = room.transform;
        troom.position = new Vector3(x * TILE_WIDTH, y * TILE_HEIGHT, z * TILE_DEPTH);

        Transform floors = new GameObject("floors").transform;
        floors.SetParent(troom);
        for (int tx = 0; tx < this.floors.GetLength(0); tx++)
            for (int ty = 0; ty < this.floors.GetLength(1); ty++)
                if (this.floors[tx, ty] != null)
                {
                    GameObject ob = Object.Instantiate(this.floors[tx, ty].gameObject, floors);
                    ob.transform.Translate(tx * TILE_WIDTH, y * TILE_HEIGHT, ty * TILE_DEPTH);
                }
        Transform ceilings = new GameObject("ceilings").transform;
        ceilings.SetParent(troom);
        for (int tx = 0; tx < this.ceilings.GetLength(0); tx++)
            for (int ty = 0; ty < this.ceilings.GetLength(1); ty++)
                if (this.ceilings[tx, ty] != null)
                {
                    GameObject ob = Object.Instantiate(this.ceilings[tx, ty].gameObject, ceilings);
                    ob.transform.Translate(tx * TILE_WIDTH, y * TILE_HEIGHT, ty * TILE_DEPTH);
                }

        Transform walls = new GameObject("walls").transform;
        walls.SetParent(troom);
        for (int tx = 0; tx < this.walls.GetLength(0); tx++)
            for (int ty = 0; ty < this.walls.GetLength(1); ty++)
                if (this.walls[tx, ty] != null)
                {
                    GameObject go = Object.Instantiate(this.walls[tx, ty].gameObject, walls);
                    go.transform.Translate(tx * TILE_WIDTH, y * TILE_HEIGHT, ty * TILE_DEPTH);

                    if ((tx == 0 || tx == 1) && (ty == 0 || ty == 1))
                    {
                        GameObject go1 = Object.Instantiate(this.walls[tx, ty].gameObject, walls);
                        go1.transform.Translate(tx * TILE_WIDTH, y * TILE_HEIGHT, ty * TILE_DEPTH);
                        go.transform.Rotate(new Vector3(0, ty == 0 ? 90 : -90, 0));
                        go1.transform.Rotate(new Vector3(0, tx == 0 ? 0 : 180, 0));
                    }
                    else
                    {
                        if (ty == 0)
                            go.transform.Rotate(new Vector3(0, 90, 0));
                        else if (ty == 1)
                            go.transform.Rotate(new Vector3(0, -90, 0));
                        else if (tx == 1)
                            go.transform.Rotate(new Vector3(0, 180, 0));
                    }
                }

        Transform components = new GameObject("components").transform;
        components.SetParent(troom);
        foreach(var component in this.components)
            Object.Instantiate(component.component.gameObject, new Vector3((component.x + x) * TILE_WIDTH, y * TILE_HEIGHT, (component.y + y) * TILE_DEPTH), Quaternion.identity, components);

        // TODO zoners and enemies

        return room;
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
