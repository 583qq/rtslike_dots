using System;
using System.Collections;
using System.Collections.Generic;

using Unity;
using Unity.Entities;
using Unity.Collections;

using Unity.Rendering;

using UnityEngine;


public class ConstructBuilding : MonoBehaviour
{
    private EntityManager manager;
    private World world;

    void Start()
    {
        world = World.DefaultGameObjectInjectionWorld;
        manager = world.EntityManager;
    }

    // Resync
    private void Awake()
    {
        world = World.DefaultGameObjectInjectionWorld;
        manager = world.EntityManager;

        DisableBuilding();
    }

    public void ActivateBuilding()
    {
        EnableBuilding();
    }

    private void EnableBuilding()
    {
        world.GetExistingSystem<BuildingSystem>().Enabled = true;
    }

    private void DisableBuilding()
    {
        world.GetExistingSystem<BuildingSystem>().Enabled = false;
    }
}