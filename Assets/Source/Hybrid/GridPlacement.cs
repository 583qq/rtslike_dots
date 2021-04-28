using System;

using Unity;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;

using UnityEngine;


public class GridPlacement : MonoBehaviour
{
    private float2 gridCellWorldSize = new float2 { x = 2, y = 2};
    private int2 gridResolution = new int2 { x = 16, y = 16};   // Cell Size

    private Entity[,] gridArray;

    private World world;
    private EntityManager manager;

    void Awake()
    {
        int width = 64, height = 64;

        gridArray = new Entity[width, height];

        // Create NONE entity
        world = World.DefaultGameObjectInjectionWorld;
        manager = world.EntityManager;

        // Fill our grid
        // Grid only for our buildings
    }
}

// Theres none
public struct GridNone : IComponentData
{
}

// Something is here
public struct GridReference : IComponentData
{
    public Entity reference;
}