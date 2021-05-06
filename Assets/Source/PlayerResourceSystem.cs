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

namespace Game 
{
public class PlayerResourceSystem : SystemBase
{
    public int startResourceValue = 100;

    private Entity player;
    

    // UI text fields references (Can use ConcurrentDictionary?)
    public Dictionary<ResourceTypes, Text> resources;
    
    private EndSimulationEntityCommandBufferSystem endSimulationECBSystem;


    protected override void OnCreate()
    {
        endSimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Initialize our player data
        player = ecb.CreateEntity();

        resources = new Dictionary<ResourceTypes, Text>();  // UI textfield mapping

        DynamicBuffer<PlayerResourceData> playerResources = ecb.AddBuffer<PlayerResourceData>(player);

        ecb.AddComponent<PlayerTag>(player);

        #region Initialize player resources (buffer)
        
        AddStartResources(playerResources, new ResourceTypes[] 
        {
            ResourceTypes.Gold,
            ResourceTypes.Wood,
            ResourceTypes.Iron,
            ResourceTypes.Crystal 
        });

        #endregion       

        #if UNITY_EDITOR
        EntityManager.SetName(player, "Player Resource Data");
        #endif

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    private void AddStartResources(DynamicBuffer<PlayerResourceData> buffer, ResourceTypes[] types)
    {
        foreach(var resource_type in types)
            buffer.Add(new PlayerResourceData
            {   
                resource = new ResourceData 
                {
                    type = resource_type,
                    value = startResourceValue
                }
            });
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
            
            // No collisions please
            resources.Add(typeTag.ResourceType, textField);
        }
        #endregion
    }

    protected override void OnUpdate()
    {
        // To-do: Refactor
        // var ecbParallel = endSimulationECBSystem.CreateCommandBuffer().AsParallelWriter();

        // UI Text Update

        Entities
                .WithoutBurst()
                .ForEach(
                    (in DynamicBuffer<PlayerResourceData> resourcesData) =>
                    {
                        for(int i = 0; i < resourcesData.Length; i++)
                        {
                            // If it's resource None type
                            if(resourcesData[i].resource.type == ResourceTypes.None)
                                continue;
                            
                            // If there is no UI text field for our resource type
                            if(!resources.ContainsKey(resourcesData[i].resource.type))
                                continue;

                            SetResourceText(resourcesData[i].resource.type, resourcesData[i].resource.value);
                        }
                    }
                ).Run();

    }

    public bool ValidateResourceSpending(ResourceTypes type, int val)
    {
        bool check = true;
        // Race condition? Leak?

        Entities
                .ForEach(
                    (in DynamicBuffer<PlayerResourceData> buffer) =>
                    {
                        foreach(var current in buffer)
                        {
                            if(current.resource.type == type)
                                if(current.resource.value < val)
                                {
                                    check = false;
                                    return;
                                }
                        }
                    }
                ).Run();

        return check;
    }

    public void SpendResource(ResourceTypes types, int val)
    {
        // Race condition? Leak? JobTempAlloc?
        // var ecbParallel = endSimulationECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
                .ForEach(
                    (ref DynamicBuffer<PlayerResourceData> buffer) =>
                    {
                        for(int i = 0; i < buffer.Length; i++)
                            if(buffer[i].resource.type == types)
                                buffer[i] = new PlayerResourceData {
                                    resource = new ResourceData
                                    {
                                        type = types,
                                        value = buffer[i].resource.value - val
                                    }
                                };
                    }
                ).ScheduleParallel();


        this.CompleteDependency();
    }

    public void AddResource(ResourceTypes type, int val)
    {
        SpendResource(type, -val);
    }

    private void SetResourceText(ResourceTypes type, int val)
    {
        resources[type].text = val.ToString();
    }
}

}