using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Weighted_Randomizer;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject player;

    [Header("Dungeon")]
    [Tooltip("The dungeon layout prefab")]
    public Dungeon dungeon;
    [Tooltip("The name of the dungeon")]
    public string dungeonName;
    [Tooltip("The seed of the dungeon generator (random at zero)")]
    public int dungeonSeed;

    [Header("Levels")]
    public int numOfLevels = 2;
    public int roomsPerLevel = 50;

    [Header("Size")]
    public int minRoomWidth = 3;
    public int minRoomDepth = 3;

    public int maxRoomWidth = 6;
    public int maxRoomDepth = 6;

    [Header("Positioning")]
    public int maxBranches = 10;
    public int minRoomGap = 1;
    public int maxRoomGap = 4;

    [Header("Connections")]
    [Tooltip("The chance that the rooms will connect inter-branch (percentage)")]
    public int interbranchConnectChance = 25;

    private GameObject genScreen;
    private GenStatusUpdater statusUpdater;

    private List<DungeonRoom>[] levels;

    private string sceneName;
    private Scene scene;

    private I18n i18n = I18n.Get();

    private System.Random random;
    private IWeightedRandomizer<DungeonTile> floors;
    private IWeightedRandomizer<DungeonTile> ceilings;
    private IWeightedRandomizer<DungeonTile> walls;
    private IWeightedRandomizer<DungeonTile> doorways;
    private IWeightedRandomizer<DungeonZoner> zoners;
    private IWeightedRandomizer<EntityEnemy> enemies;
    private IWeightedRandomizer<DungeonTile> connectors;

    private void Awake()
    {
        if (dungeon)
        {
            dungeon = Instantiate(dungeon.gameObject, null).GetComponent<Dungeon>();
            DontDestroyOnLoad(dungeon.gameObject);
        }
    }

    private void ConfigureRandom()
    {
        if (dungeonSeed == 0)
            dungeonSeed = new System.Random().Next(int.MinValue, int.MaxValue);
        random = new System.Random(dungeonSeed);

        floors = new DynamicWeightedRandomizer<DungeonTile>(dungeonSeed);
        ceilings = new DynamicWeightedRandomizer<DungeonTile>(dungeonSeed);
        walls = new DynamicWeightedRandomizer<DungeonTile>(dungeonSeed);
        doorways = new DynamicWeightedRandomizer<DungeonTile>(dungeonSeed);
        zoners = new DynamicWeightedRandomizer<DungeonZoner>(dungeonSeed);
        enemies = new DynamicWeightedRandomizer<EntityEnemy>(dungeonSeed);
        connectors = new DynamicWeightedRandomizer<DungeonTile>(dungeonSeed);

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
        foreach (DungeonTile connector in dungeon.connectors)
            connectors[connector] = connector.weight;
    }

    private void Start()
    {
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

        StartCoroutine(GenerateScene());
    }

    private IEnumerator GenerateScene()
    {
        ConfigureRandom();

        yield return SetStatus("planning");

        levels = new List<DungeonRoom>[numOfLevels];

        for (int i = 0; i < levels.Length; i++)
            levels[i] = new List<DungeonRoom>();

        for (int floor = 0; floor < numOfLevels; floor++)
        {
            //print("Generating floor " + floor);
            for (int rc = 0; rc < roomsPerLevel; rc++)
            {
                yield return null;
                //print("Generating room " + (rc + 1) + " of " + floorRooms + "(" + floor + ")");

                int rw = random.Next(minRoomWidth, maxRoomWidth);
                int rh = random.Next(minRoomDepth, maxRoomDepth);

                DungeonRoom room = new DungeonRoom(rw, rh);

                levels[floor].Add(room);

                for (int tx = 0; tx < rw; tx++)
                    for (int ty = 0; ty < rh; ty++)
                    {
                        room.floors[tx, ty] = floors.NextWithReplacement();
                        room.ceilings[tx, ty] = ceilings.NextWithReplacement();

                        if (tx == 0 || ty == 0 || tx == room.width - 1 || ty == room.height - 1)
                            room.walls[tx, ty] = walls.NextWithReplacement();
                    }

                foreach (DungeonComponent comp in dungeon.components)
                {
                    comp.Place(room);
                }
            }
        }

        yield return SetStatus("shuffling");

        for (int i = 0; i < levels.Length; i++)
            levels[i] = levels[i].OrderBy(a => Guid.NewGuid()).ToList();

        yield return SetStatus("positioning");

        for (int level = 0; level < levels.Length; level++)
        {
            List<Branch> branches = new List<Branch>();
            List<DungeonRoom> rooms = levels[level];

            int cr = 0;
            int curY = 0;

            while (cr < rooms.Count)
            {
                yield return null;

                int curX = 0;
                int tallestRoom = 0;

                rooms[cr].position = new IntCoords3(curX, level, curY);
                DungeonRoom branchParent = rooms[cr];
                DungeonRoom lastRoom = rooms[cr];

                Branch branch = new Branch(branchParent);
                branches.Add(branch);

                if (rooms[cr].height > tallestRoom)
                    tallestRoom = rooms[cr].height;

                cr++;

                int leftRooms = random.Next(0, maxBranches);
                int rightRooms = random.Next(0, maxBranches);

                for (int x = 0; x < leftRooms && cr < rooms.Count; x++)
                {
                    curX -= random.Next(minRoomGap, maxRoomGap) + rooms[cr].width;

                    rooms[cr].position = new IntCoords3(curX, level, curY);

                    if (lastRoom != null)
                        rooms[cr].connections.Add(lastRoom);

                    if (rooms[cr].height > tallestRoom)
                        tallestRoom = rooms[cr].height;

                    branch.leftRooms.Add(rooms[cr]);

                    lastRoom = rooms[cr];
                    cr++;
                }

                curX = branchParent.width;
                lastRoom = branchParent;

                for (int x = 0; x < rightRooms && cr < rooms.Count; x++)
                {
                    curX += random.Next(minRoomGap, maxRoomGap);

                    rooms[cr].position = new IntCoords3(curX, level, curY);
                    curX += rooms[cr].width;

                    if (lastRoom != null)
                        rooms[cr].connections.Add(lastRoom);

                    if (rooms[cr].height > tallestRoom)
                        tallestRoom = rooms[cr].height;

                    branch.rightRooms.Add(rooms[cr]);

                    lastRoom = rooms[cr];
                    cr++;
                }

                curY += random.Next(minRoomGap, maxRoomGap) + tallestRoom;
            }

            for (int br = 1; br < branches.Count; br++)
            {
                branches[br].centerRoom.connections.Add(branches[br - 1].centerRoom);
            }
        }

        yield return SetStatus("connecting");

        for (int level = 0; level < levels.Length; level++)
        {
            List<DungeonRoom> connectorRooms = new List<DungeonRoom>();
            foreach (DungeonRoom room in from d in levels[level]
                                         where d.connections.Count >= 1
                                         select d)
            {
                foreach (DungeonRoom target in room.connections)
                {
                    IntCoords3 roomPosition = room.position.Value;
                    IntCoords3 targetPosition = target.position.Value;

                    CardinalDirection direction;
                    int xdif = Mathf.Abs(roomPosition.x - targetPosition.x);
                    int ydif = Mathf.Abs(roomPosition.z - targetPosition.z);

                    if (xdif > ydif)
                        if (roomPosition.x > targetPosition.z)
                            direction = CardinalDirection.WEST;
                        else
                            direction = CardinalDirection.EAST;
                    else if (xdif < ydif)
                        if (roomPosition.y > targetPosition.z)
                            direction = CardinalDirection.SOUTH;
                        else
                            direction = CardinalDirection.NORTH;
                    else
                        throw new Exception("The room we're trying to connect to is diagonal to us?");

                    switch (direction)
                    {
                        //TODO other side wall replacement
                        case CardinalDirection.NORTH:
                            {
                                room.walls[room.width / 2, 0] = doorways.NextWithReplacement();
                                //TODO fix math
                                DungeonRoom connectorRoom = new ConnectorRoom(1, Math.Abs(roomPosition.z - (targetPosition.z + target.height)));
                                connectorRoom.position = new IntCoords3(roomPosition.x + room.width / 2, level, targetPosition.z + target.height);
                                connectorRooms.Add(connectorRoom);
                                break;
                            }
                        case CardinalDirection.SOUTH:
                            {
                                room.walls[room.width / 2, room.height - 1] = doorways.NextWithReplacement();
                                //TODO math
                                break;
                            }
                        case CardinalDirection.EAST:
                            {
                                room.walls[room.width - 1, room.height / 2] = doorways.NextWithReplacement();
                                //TODO math
                                break;
                            }
                        case CardinalDirection.WEST:
                            {
                                room.walls[0, room.height / 2] = doorways.NextWithReplacement();
                                //TODO math
                                break;
                            }
                    }
                }
            }

            foreach (DungeonRoom room in connectorRooms)
            {
                for (int x = 0; x < room.width; x++)
                    for (int y = 0; y < room.height; y++)
                        room.floors[x, y] = connectors.NextWithReplacement();
            }

            levels[level].AddRange(connectorRooms);
        }

        BuildDungeon();

        yield return new WaitForSeconds(5F);

        Destroy(dungeon.gameObject);
        Destroy(gameObject);
    }

    private void BuildDungeon()
    {
        SetStatus("building");

        GameObject dng = new GameObject("Dungeon");

        for (int level = 0; level < numOfLevels; level++)
            foreach (DungeonRoom room in levels[level])
                room.Generate(room.position.Value.x, room.position.Value.y, room.position.Value.z).transform.SetParent(dng.transform);
    }

    private void VerifyPath(DungeonRoom[,,] dungeon)
    {

    }

    private object SetStatus(string s)
    {
        s = i18n.Translate("dungeon.generator." + s);

        if (statusUpdater)
            statusUpdater.status = s;

        return null;
    }

    private struct Branch
    {
        public DungeonRoom centerRoom;
        public List<DungeonRoom> leftRooms;
        public List<DungeonRoom> rightRooms;

        public Branch(DungeonRoom center)
        {
            centerRoom = center;
            leftRooms = new List<DungeonRoom>();
            rightRooms = new List<DungeonRoom>();
        }
    }
}
