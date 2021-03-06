﻿using System;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    // Tiles
    [NonSerialized]
    public DungeonTile[] floors;
    [NonSerialized]
    public DungeonTile[] walls;
    [NonSerialized]
    public DungeonTile[] ceilings;
    [NonSerialized]
    public DungeonTile[] doorways;
    [NonSerialized]
    public DungeonZoner[] zoners;
    [NonSerialized]
    public DungeonTile[] connectors;

    // Rooms
    [NonSerialized]
    public DungeonCustomRoom[] customRooms;

    // Components
    [NonSerialized]
    public DungeonTile[] components;

    // Enemies
    [NonSerialized]
    public EntityEnemy[] enemies;

    private void Awake()
    {
        Transform tiles = transform.Find("tiles");

        if (tiles)
        {
            Transform floors = tiles.Find("floors");
            if (floors)
                this.floors = floors.GetComponentsInChildren<DungeonTile>();

            Transform walls = tiles.Find("walls");
            if (walls)
                this.walls = walls.GetComponentsInChildren<DungeonTile>();

            Transform ceilings = tiles.Find("ceilings");
            if (ceilings)
                this.ceilings = ceilings.GetComponentsInChildren<DungeonTile>();

            Transform doorways = tiles.Find("doorways");
            if (doorways)
                this.doorways = doorways.GetComponentsInChildren<DungeonTile>();

            Transform zoners = tiles.Find("zoners");
            if (zoners)
                this.zoners = zoners.GetComponentsInChildren<DungeonZoner>();

            Transform connectors = tiles.Find("connectors");
            if (connectors)
                this.connectors = connectors.GetComponentsInChildren<DungeonTile>();
        }

        Transform rooms = transform.Find("rooms");
        if (rooms)
            customRooms = rooms.GetComponentsInChildren<DungeonCustomRoom>();

        Transform components = transform.Find("components");
        if (components)
            this.components = components.GetComponentsInChildren<DungeonTile>();

        Transform enemies = transform.Find("enemies");
        if (enemies)
            this.enemies = enemies.GetComponentsInChildren<EntityEnemy>();
    }
}
