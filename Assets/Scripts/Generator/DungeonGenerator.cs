using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Weighted_Randomizer;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject playerOverride;

    public bool isDedicated = true;

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

    [Header("Spawns")]
    public int enemyGenPasses = 20;

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

    private bool generationComplete = false;

    public static Vector2[] exitsCache;

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
        if (isDedicated)
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
        }

        Thread thr = new Thread(new ThreadStart(GenerateScene));
        thr.Name = "Dungeon Generation Thread";
        generationComplete = false;
        thr.Start();
    }

    private void Update()
    {
        if (generationComplete)
        {
            generationComplete = false;
            StartCoroutine(BuildDungeon());
        }
    }

    private void GenerateScene()
    {
        ConfigureRandom();

        SetStatus("planning");

        levels = new List<DungeonRoom>[numOfLevels];

        for (int i = 0; i < levels.Length; i++)
            levels[i] = new List<DungeonRoom>();

        for (int floor = 0; floor < numOfLevels; floor++)
        {
            //print("Generating floor " + floor);
            for (int rc = 0; rc < roomsPerLevel; rc++)
            {
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

        SetStatus("shuffling");

        for (int i = 0; i < levels.Length; i++)
            levels[i] = levels[i].OrderBy(a => Guid.NewGuid()).ToList();

        SetStatus("positioning");

        for (int level = 0; level < levels.Length; level++)
        {
            List<Branch> branches = new List<Branch>();
            List<DungeonRoom> rooms = levels[level];

            int cr = 0;
            int curY = 0;

            while (cr < rooms.Count)
            {
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

                /*for (int dr = 0; dr < 2; dr++)
                {
                    List<DungeonRoom> curRooms = dr == 0 ? branches[br].leftRooms : branches[br].rightRooms;
                    List<DungeonRoom> upRooms = dr == 0 ? branches[br - 1].leftRooms : branches[br - 1].rightRooms;

                    for (int r = 0; r < curRooms.Count; r++)
                    {
                        if (r < upRooms.Count && random.NextBool(interbranchConnectChance))
                            curRooms[r].connections.Add(upRooms[r]);
                    }
                }*/
            }
        }

        SetStatus("connecting");

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
                        if (roomPosition.x > targetPosition.x)
                            direction = CardinalDirection.WEST;
                        else
                            direction = CardinalDirection.EAST;
                    else if (xdif < ydif)
                        if (roomPosition.z > targetPosition.z)
                            direction = CardinalDirection.NORTH;
                        else
                            direction = CardinalDirection.SOUTH;
                    else
                        throw new Exception("The room we're trying to connect to is diagonal to us?");

                    switch (direction)
                    {
                        case CardinalDirection.NORTH:
                            {
                                room.walls[room.width / 2, 0] = doorways.NextWithReplacement();
                                IntCoords2 targetHole = target.ToRoomPosition(roomPosition.x + room.width / 2, targetPosition.z + target.height - 1);
                                target.walls[targetHole.x, targetHole.y] = doorways.NextWithReplacement();
                                DungeonRoom connectorRoom = new ConnectorRoom(1, roomPosition.z - (targetPosition.z + target.height), false);
                                connectorRoom.position = new IntCoords3(roomPosition.x + room.width / 2, level, targetPosition.z + target.height);
                                connectorRooms.Add(connectorRoom);
                                break;
                            }
                        case CardinalDirection.SOUTH:
                            {
                                room.walls[room.width / 2, room.height - 1] = doorways.NextWithReplacement();
                                IntCoords2 targetHole = target.ToRoomPosition(targetPosition.x + target.width / 2, targetPosition.z);
                                target.walls[targetHole.x, targetHole.y] = doorways.NextWithReplacement();
                                DungeonRoom connectorRoom = new ConnectorRoom(1, targetPosition.z - (roomPosition.z + room.height), false);
                                connectorRoom.position = new IntCoords3(roomPosition.x + room.width / 2, level, roomPosition.z + room.height);
                                connectorRooms.Add(connectorRoom);
                                break;
                            }
                        case CardinalDirection.EAST:
                            {
                                room.walls[room.width - 1, room.height / 2] = doorways.NextWithReplacement();
                                IntCoords2 targetHole = target.ToRoomPosition(targetPosition.x, targetPosition.z + target.height / 2);
                                target.walls[targetHole.x, targetHole.y] = doorways.NextWithReplacement();
                                DungeonRoom connectorRoom = new ConnectorRoom(targetPosition.x - (roomPosition.x + room.width), 1, true);
                                connectorRoom.position = new IntCoords3(roomPosition.x + room.width, level, roomPosition.z + room.height / 2);
                                connectorRooms.Add(connectorRoom);
                                break;
                            }
                        case CardinalDirection.WEST:
                            {
                                room.walls[0, room.height / 2] = doorways.NextWithReplacement();
                                IntCoords2 targetHole = target.ToRoomPosition(targetPosition.x + target.width - 1, roomPosition.z + room.height / 2);
                                target.walls[targetHole.x, targetHole.y] = doorways.NextWithReplacement();
                                DungeonRoom connectorRoom = new ConnectorRoom(roomPosition.x - (targetPosition.x + target.width), 1, true);
                                connectorRoom.position = new IntCoords3(targetPosition.x + target.width, level, targetPosition.z + target.height / 2);
                                connectorRooms.Add(connectorRoom);
                                break;
                            }
                    }
                }
            }

            foreach (DungeonRoom room in connectorRooms)
            {
                DungeonTile tile = connectors.NextWithReplacement();
                for (int x = 0; x < room.width; x++)
                    for (int y = 0; y < room.height; y++)
                    {
                        room.floors[x, y] = tile;
                    }
            }

            levels[level].AddRange(connectorRooms);
        }

        generationComplete = true;
    }

    private IEnumerator BuildDungeon()
    {
        SetStatus("building");

        GameObject dungeon = new GameObject("Dungeon");

        for (int level = 0; level < numOfLevels; level++)
        {
            GameObject lvl = new GameObject(level.ToString());
            lvl.transform.SetParent(dungeon.transform);
            foreach (DungeonRoom room in levels[level])
                room.Generate(room.position.Value.x, room.position.Value.y, room.position.Value.z).transform.SetParent(lvl.transform);
        }

        SetStatus("spawning.player");

        Vector3 playerSpawn;

        {
            SpawnPoint[] playerSpawns = dungeon.transform.Find("0").GetComponentsInChildren<SpawnPoint>();
            SpawnPoint playerSpawnPoint = random.NextFrom(playerSpawns);
            playerSpawn = playerSpawnPoint.transform.position + playerSpawnPoint.offset;

            foreach (SpawnPoint spawn in playerSpawnPoint.transform.parent.GetComponentsInChildren<SpawnPoint>())
                DestroyImmediate(spawn);
        }

        SetStatus("spawning.teleporters");

        exitsCache = new Vector2[levels.Length];

        for (int level = 0; level < levels.Length - 1; level++)
        {
            SpawnPoint[] lowerSpawns = dungeon.transform.Find(level.ToString()).GetComponentsInChildren<SpawnPoint>();
            SpawnPoint[] upperSpawns = dungeon.transform.Find((level + 1).ToString()).GetComponentsInChildren<SpawnPoint>();

            SpawnPoint lowerSpawn = random.NextFrom(lowerSpawns);
            SpawnPoint upperSpawn = random.NextFrom(upperSpawns);

            DungeonZoner template = zoners.NextWithReplacement();

            GameObject lowerZonerObject = Instantiate(template.gameObject, lowerSpawn.transform.parent, false);
            GameObject upperZonerObject = Instantiate(template.gameObject, upperSpawn.transform.parent, false);

            lowerZonerObject.transform.position = lowerSpawn.transform.position + lowerSpawn.offset;
            upperZonerObject.transform.position = upperSpawn.transform.position + upperSpawn.offset;

            DungeonZoner lowerZoner = lowerZonerObject.GetComponent<DungeonZoner>();
            DungeonZoner upperZoner = upperZonerObject.GetComponent<DungeonZoner>();

            lowerZoner.zonerLink = upperZoner.transform.position;
            upperZoner.zonerLink = lowerZoner.transform.position;

            exitsCache[level] = new Vector2(lowerZoner.transform.position.x, lowerZoner.transform.position.z);

            print(level + ": " + lowerZoner.transform.position);
        }

        SetStatus("spawning.enemies");

        Transform enemiesParent = new GameObject("enemies").transform;
        for (int pass = 0; pass < enemyGenPasses; pass++)
        {
            for (int pass2 = 0; pass2 < roomsPerLevel * numOfLevels - random.Next(2, roomsPerLevel); pass2++)
            {
                SpawnPoint spawn = random.NextFrom(dungeon.transform.GetComponentsInChildren<SpawnPoint>());

                if (!spawn)
                    break;

                EntityEnemy enemy = enemies.NextWithReplacement();

                GameObject enemyInst = Instantiate(enemy.gameObject, enemiesParent);
                enemyInst.transform.position = spawn.transform.position + spawn.offset;

                enemyInst.GetComponent<EntityEnemy>().OnSpawn();

                Destroy(spawn);
            }
        }

        SetStatus("spawning.treasure");

        //TODO summon treasure

        SetStatus("spawning.exit");

        //TODO summon exit

        SetStatus("wait");

        if (isDedicated)
            yield return new WaitForSeconds(4F); // Wait a bit because we're too fast to see my loading animation :/

        Destroy(this.dungeon.gameObject);
        Destroy(gameObject);


        if (playerOverride)
            Instantiate(playerOverride, playerSpawn, Quaternion.identity);
        else
        {
            if (PlayerData.Instance == null)
                PlayerData.Instance = new PlayerData("#Debug Player", PlayerData.PlayerClass.MAGE);

            Instantiate(Resources.Load<GameObject>(PlayerData.prefabs[PlayerData.Instance.playerClass]), playerSpawn, Quaternion.identity);
        }
        if (isDedicated)
        {
            GameObject mods = new GameObject("modules");
            foreach (UnityEngine.Object ob in Resources.LoadAll("Modules/"))
            {
                if (ob is GameObject)
                {
                    GameObject mod = Instantiate((GameObject)ob);
                    mod.transform.SetParent(mods.transform);
                }
            }

            GameObject gl = new GameObject("Global Light");
            Light globalLight = gl.AddComponent<Light>();
            globalLight.type = LightType.Directional;
            gl.transform.eulerAngles = new Vector3(50, -30, 0); // TODO proper lighting
        }
    }

    private void VerifyPath(DungeonRoom[,,] dungeon)
    {

    }

    private void SetStatus(string s)
    {
        if (!isDedicated)
            return;

        s = i18n.Translate("dungeon.generator." + s);

        if (statusUpdater)
            statusUpdater.status = s;
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
