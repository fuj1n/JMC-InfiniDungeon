using System.Collections.Generic;
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

        GenerateScene();
    }

    private void GenerateScene()
    {
        SetStatus("dungeon.generator.planning");

        List<DungeonRoom>[] rooms = new List<DungeonRoom>[numOfFloors];

        for (int i = 0; i < rooms.Length; i++)
            rooms[i] = new List<DungeonRoom>();

        System.Random random = new System.Random();

        for (int floor = 0; floor < numOfFloors; floor++)
        {
            print("Generating floor " + floor);
            for (int rc = 0; rc < floorRooms; rc++)
            {
                print("Generating room " + (rc + 1) + " of " + floorRooms + "(" + floor + ")");

                int rw = random.Next(minRoomWidth, maxRoomWidth);
                int rh = random.Next(minRoomDepth, maxRoomDepth);

                DungeonRoom room = new DungeonRoom(rw, rh);

                rooms[floor].Add(room);

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

                // TODO generate enemies
            }
        }

        /*for (int floor = 0; floor < numOfFloors - 1; floor++)
        {
            int fn = floor * 2;

            DungeonRoom selectedRoom;

            List<DungeonRoom> possibilities = rooms[floor];

            if (possibilities.Count < 1)
                throw new IndexOutOfRangeException("No valid rooms generated for floor " + floor + "?");

            selectedRoom = possibilities[random.Next(0, possibilities.Count)];

            DungeonRoom targetRoom = spawnRooms[fn + 1];

            DungeonZoner zoner = zoners.NextWithReplacement();
            zoner.Place(selectedRoom, targetRoom, random);
        }*/

        SetStatus("dungeon.generator.building");

        GameObject dng = new GameObject("Dungeon");

        for (int floor = 0; floor < numOfFloors; floor++)
            foreach (DungeonRoom room in rooms[floor])
            {
                //room.Generate(room.Key.x, room.Key.y, room.Key.z).transform.SetParent(dng.transform);
            }

        Destroy(dungeon);
    }

    private void VerifyPath(DungeonRoom[,,] dungeon)
    {

    }

    private void SetStatus(string s)
    {
        s = i18n.Translate(s);

        if (statusUpdater)
            statusUpdater.status = s;
    }
}
