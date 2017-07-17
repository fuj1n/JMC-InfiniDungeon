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
    [Tooltip("The chance that the rooms will connect intra-branch (percentage)")]
    public int intrabranchConnectChance = 75;
    [Tooltip("Whether an inter-branch connection will be guaranteed when a connection is missing")]
    public bool guaranteeConnection = true;
    [Tooltip("Whether the generator should prevent isolated rooms")]
    public bool isolationPrevention = true;

    private GameObject genScreen;
    private GenStatusUpdater statusUpdater;

    private string sceneName;
    private Scene scene;

    private I18n i18n = I18n.Create();

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

        StartCoroutine(GenerateScene());
    }

    private IEnumerator GenerateScene()
    {
        yield return SetStatus("planning");

        List<DungeonRoom>[] levels = new List<DungeonRoom>[numOfLevels];

        for (int i = 0; i < levels.Length; i++)
            levels[i] = new List<DungeonRoom>();

        System.Random random = new System.Random();

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
            List<DungeonRoom> rooms = levels[level];

            int cr = 0;
            int curY = 0;

            while (cr < rooms.Count)
            {
                yield return null;

                int curX = 0;
                int tallestRoom = 0;

                rooms[cr].position = new IntCoords3(curX, level, curY);
                curX += rooms[cr].width;

                if (rooms[cr].height > tallestRoom)
                    tallestRoom = rooms[cr].height;

                cr++;

                int leftRooms = random.Next(0, maxBranches);
                int rightRooms = random.Next(0, maxBranches);

                for (int x = 0; x < leftRooms && cr < rooms.Count; x++)
                {
                    curX -= random.Next(minRoomGap, maxRoomGap) + rooms[cr].width;

                    rooms[cr].position = new IntCoords3(curX, level, curY);

                    if (rooms[cr].height > tallestRoom)
                        tallestRoom = rooms[cr].height;

                    cr++;
                }

                curX = 0;

                for (int x = 0; x < rightRooms && cr < rooms.Count; x++)
                {
                    curX += random.Next(minRoomGap, maxRoomGap);

                    rooms[cr].position = new IntCoords3(curX, level, curY);
                    curX += rooms[cr].width;

                    if (rooms[cr].height > tallestRoom)
                        tallestRoom = rooms[cr].height;

                    cr++;
                }

                curY += random.Next(minRoomGap, maxRoomGap) + tallestRoom;
            }
        }

        yield return SetStatus("building");

        GameObject dng = new GameObject("Dungeon");

        for (int level = 0; level < numOfLevels; level++)
            foreach (DungeonRoom room in levels[level])
            {
                yield return null;
                room.Generate(room.position.Value.x, room.position.Value.y, room.position.Value.z).transform.SetParent(dng.transform);
            }

        yield return new WaitForSeconds(5F);

        Destroy(dungeon.gameObject);
        Destroy(gameObject);
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
}
