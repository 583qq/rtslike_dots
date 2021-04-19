using System;
using System.Collections.Generic;

using Unity;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;
using UnityEngine.UI;

public class PlayerResourceSystem : SystemBase
{
    public int startResourceValue = 100;

    private Entity player;
    
    // UI text fields references
    private Dictionary<ResourceTypes, Text> resources;

    private bool startValueSet = false;

    protected override void OnCreate()
    {
        // Initialize our player data
        player = EntityManager.CreateEntity();
        resources = new Dictionary<ResourceTypes, Text>();

        // Eternal damnation
        EntityManager.AddComponentData<PlayerData>(player, new PlayerData {
            Gold = startResourceValue,
            Wood = startResourceValue,
            Iron = startResourceValue,
            Crystal = startResourceValue
        });

        #if UNITY_EDITOR
        EntityManager.SetName(player, "Player Resource Data");
        #endif
    }

    protected override void OnStartRunning()
    {
        #region Resource UI fields 
        var textQuery = GetEntityQuery(typeof(PriceDataTag), typeof(Text));

        if(textQuery.IsEmpty)
        {
            Debug.Log("Text fields for resources were not found.");
        }

        var textEntityArray = textQuery.ToEntityArray(Allocator.Temp);

        foreach(var entity in textEntityArray)
        {
            var typeTag = EntityManager.GetComponentData<PriceDataTag>(entity);
            var textField = EntityManager.GetComponentObject<Text>(entity);
            
            // Debug.Log($"Adding {typeTag.ResourceType} field.");
            // No collisions please
            resources.Add(typeTag.ResourceType, textField);
        }
        #endregion

        if(startValueSet)
            return;

        startValueSet = true;
    }

    protected override void OnUpdate()
    {
        // Update UI
        
        // THIS IS BAD

        Entities
                .WithoutBurst()
                .ForEach(
                    (in PlayerData player) =>
                    {
                        SetResourceText(ResourceTypes.Gold, player.Gold);
                        SetResourceText(ResourceTypes.Wood, player.Wood);
                        SetResourceText(ResourceTypes.Iron, player.Iron);
                        SetResourceText(ResourceTypes.Crystal, player.Crystal);
                    }
                ).Run();
    }

    private void SetResourceText(ResourceTypes type, int value)
    {
        resources[type].text = value.ToString();
    }
}