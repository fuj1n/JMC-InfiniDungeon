using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Weighted_Randomizer;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject player;

    [Header("Dungeon")]
    public Dungeon dungeon;
    public string dungeonName;

    [Header("Floors")]
    public int numOfFloors = 2;
    public int floorRooms = 25;

    [Header("Size")]
    public int minRoomWidth = 3;
    public int minRoomDepth = 3;

    public int maxRoomWidth = 6;
    public int maxRoomDepth = 6;

    private GameObject genScreen;
    private GenStatusUpdater statusUpdater;

    private string sceneName;
    private Scene scene;

    private IWeightedRandomizer<DungeonTile> floors = new DynamicWeightedRandomizer<DungeonTile>();
    private IWeightedRandomizer<DungeonTile> ceilings = new DynamicWeightedRandomizer<DungeonTile>();
    private IWeightedRandomizer<DungeonTile> walls = new DynamicWeightedRandomizer<DungeonTile>();
    private IWeightedRandomizer<DungeonTile> doorways = new DynamicWeightedRandomizer<DungeonTile>();
    private IWeightedRandomizer<DungeonZoner> zoners = new DynamicWeightedRandomizer<DungeonZoner>();

    private IWeightedRandomizer<EntityEnemy> enemies = new DynamicWeightedRandomizer<EntityEnemy>();

    private void Awake()
    {
        if (dungeon)
        {
            dungeon = Instantiate(dungeon.gameObject, null).GetComponent<Dungeon>();
            DontDestroyOnLoad(dungeon.gameObject);
        }
    }

    private void Start()
    {
        foreach (DungeonTile floor in dungeon.floors)
            floors[floor] = floor.weight;
        foreach (DungeonTile ceiling in dungeon.ceilings)
            ceilings[ceiling] = ceiling.weight;
        foreach (DungeonTile wall in dungeon.walls)
            walls[wall] = wall.weight;
        foreach (DungeonTile doorway in dungeon.doorways)
            doorways[doorway] = doorway.weight;
        foreach (DungeonZoner zoner in dungeon.zoners)
            zoners[zoner] = zoner.weight;
        foreach (EntityEnemy enemy in dungeon.enemies)
            enemies[enemy] = enemy.weight;

        sceneName = "Generated/" + dungeonName;

        transform.SetParent(null, true);

        genScreen = Resources.Load<GameObject>("Generator/Loader");
        if (genScreen)
        {
            genScreen = Instantiate(genScreen);
            genScreen.transform.SetParent(transform);

            statusUpdater = genScreen.GetComponentInChildren<GenStatusUpdater>();
        }

        DontDestroyOnLoad(gameObject);
        scene = SceneManager.CreateScene(sceneName);

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i) != scene)
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
        }

        SceneManager.SetActiveScene(scene);

        GenerateScene();
    }

    private void GenerateScene()
    {
        SetStatus("dungeon.generator.planning");

        Dictionary<IntCoords3, DungeonRoom> gen = new Dictionary<IntCoords3, DungeonRoom>();
        System.Random random = new System.Random();

        DungeonRoom[] spawnRooms = new DungeonRoom[numOfFloors];

        Dictionary<DungeonRoom.Placement<DungeonTile>, CardinalDirection> availableExits = new Dictionary<DungeonRoom.Placement<DungeonTile>, CardinalDirection>();

        for (int floor = 0; floor < numOfFloors; floor++)
        {
            print("Generating floor " + floor);
            for (int rc = 0; rc < floorRooms && (availableExits.Count > 0 || rc == 0); rc++)
            {
                print("Generating room " + rc + " of " + floorRooms);

                CardinalDirection? requireSide = null;
                IntCoords2? requireCoords = null;

                int x = 0;
                int y = 0;
                if (availableExits.Count > 0)
                {
                    int exit = random.Next(0, availableExits.Count);
                    var place = availableExits.Keys.ElementAt(exit);
                    CardinalDirection side = availableExits[place];

                    switch (side)
                    {
                        case CardinalDirection.NORTH:
                            x = (int)place.x;
                            y = (int)place.y - 1;
                            requireSide = CardinalDirection.SOUTH;
                            break;
                        case CardinalDirection.SOUTH:
                            x = (int)place.x;
                            y = (int)place.y + 1;
                            requireSide = CardinalDirection.NORTH;
                            break;
                        case CardinalDirection.EAST:
                            x = (int)place.x + 1;
                            y = (int)place.y;
                            requireSide = CardinalDirection.WEST;
                            break;
                        case CardinalDirection.WEST:
                            x = (int)place.x - 1;
                            y = (int)place.y;
                            requireSide = CardinalDirection.EAST;
                            break;
                    }

                    requireCoords = new IntCoords2(x, y);

                    availableExits.Remove(place);
                }

                int rw = random.Next(minRoomWidth, maxRoomWidth);
                int rh = random.Next(minRoomDepth, maxRoomDepth);

                if (requireSide != null)
                {
                    switch (requireSide.Value)
                    {
                        case CardinalDirection.NORTH:
                            x -= rw / 2;
                            y += (rh - 1);
                            break;
                        case CardinalDirection.SOUTH:
                            x -= rw / 2;
                            break;
                        case CardinalDirection.EAST:
                            y -= rh / 2;
                            break;
                        case CardinalDirection.WEST:
                            x -= (rw - 1);
                            y -= rh / 2;
                            break;
                    }
                }

                DungeonRoom room = new DungeonRoom(x, floor, y, rw, rh);

                bool skip = false;

                for (int wx = 0; wx < rw; wx++)
                    for (int wy = 0; wy < rh; wy++)
                        if (gen.ContainsKey(new IntCoords3(wx + x, floor, wy + y)))
                            skip = true;

                if (skip)
                    print("Skipped");

                if (skip)
                    continue;

                for (int wx = 0; wx < rw; wx++)
                    for (int wy = 0; wy < rh; wy++)
                        gen.Add(new IntCoords3(wx + x, floor, wy + y), room);

                int numDoorways = random.Next(1, 5);

                if (rc == 0)
                {
                    spawnRooms[floor] = room;
                    room.isSpawn = true;
                    numDoorways = 1;
                }
                else
                {
                    room.isSpawn = false;
                }

                for (int tx = 0; tx < rw; tx++)
                    for (int ty = 0; ty < rh; ty++)
                    {
                        room.floors[tx, ty] = floors.NextWithReplacement();
                        room.ceilings[tx, ty] = ceilings.NextWithReplacement();

                        if (tx == 0 || ty == 0 || tx == room.width - 1 || ty == room.height - 1)
                            room.walls[tx, ty] = walls.NextWithReplacement();
                    }

                List<CardinalDirection> cardinals = Enum.GetValues(typeof(CardinalDirection)).Cast<CardinalDirection>().ToList();

                if (numDoorways > cardinals.Count)
                    throw new IndexOutOfRangeException("The number of doorways is larger than the number of cardinal directions");

                for (int dw = 0; dw < numDoorways; dw++)
                {
                    int chosenDir = random.Next(0, cardinals.Count);

                    if (requireSide != null)
                    {
                        chosenDir = (int)requireSide.Value;
                        requireSide = null;
                    }

                    CardinalDirection side = cardinals[chosenDir];
                    cardinals.RemoveAt(chosenDir);

                    int sx, sy;

                    switch (side)
                    {
                        case CardinalDirection.NORTH:
                            sx = random.Next(0, room.width);
                            sy = 0;
                            break;
                        case CardinalDirection.SOUTH:
                            sx = random.Next(0, room.width);
                            sy = room.height - 1;
                            break;
                        case CardinalDirection.WEST:
                            sx = 0;
                            sy = random.Next(0, room.height);
                            break;
                        case CardinalDirection.EAST:
                            sx = room.width - 1;
                            sy = random.Next(0, room.height);
                            break;
                        default:
                            throw new IndexOutOfRangeException("Illegal cardinal direction?");
                    }

                    if (requireCoords != null)
                    {
                        sx = requireCoords.Value.x;
                        sy = requireCoords.Value.y;
                    }

                    room.walls[sx, sy] = doorways.NextWithReplacement();
                    room.exits[side] = new DungeonRoom.Placement<DungeonTile>(room, sx, sy, room.walls[sx, sy]);
                    availableExits[new DungeonRoom.Placement<DungeonTile>(room, x + sx, y + sy, room.walls[sx, sy])] = side;
                }

                foreach (DungeonComponent comp in dungeon.components)
                {
                    comp.Place(room);
                }

                // TODO generate enemies
            }
        }

        for (int floor = 0; floor < numOfFloors - 1; floor++)
        {
            int fn = floor * 2;

            DungeonRoom selectedRoom;

            List<DungeonRoom> possibilities = new List<DungeonRoom>();

            foreach (KeyValuePair<IntCoords3, DungeonRoom> room in gen)
            {
                if (!room.Value.isSpawn && room.Value.IsRoot(room.Key.x, room.Key.y, room.Key.z))
                    possibilities.Add(room.Value);
            }

            if (possibilities.Count < 1)
                throw new IndexOutOfRangeException("No valid rooms generated?");

            selectedRoom = possibilities[random.Next(0, possibilities.Count)];

            DungeonRoom targetRoom = spawnRooms[fn + 1];

            DungeonZoner zoner = zoners.NextWithReplacement();
            zoner.Place(selectedRoom, targetRoom, random);
        }

        // Remove unused exits
        foreach (var exit in availableExits)
        {
            var loc = exit.Key.parent.exits[exit.Value];

            exit.Key.parent.walls[(int)loc.x, (int)loc.y] = walls.NextWithReplacement();

            exit.Key.parent.exits.Remove(exit.Value);
        }
        availableExits.Clear();

        SetStatus("dungeon.generator.building");

        GameObject dng = new GameObject("Dungeon");

        foreach (KeyValuePair<IntCoords3, DungeonRoom> room in gen)
        {
            if (room.Value.IsRoot(room.Key.x, room.Key.y, room.Key.z))
                room.Value.Generate(room.Key.x, room.Key.y, room.Key.z).transform.SetParent(dng.transform);
        }

        Destroy(dungeon);
    }

    private void VerifyPath(DungeonRoom[,,] dungeon)
    {

    }

    private void SetStatus(string s)
    {
        if (statusUpdater)
            statusUpdater.status = s;
    }
}
