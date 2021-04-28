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
    public Dictionary<ResourceTypes, Text> resources;
    
    // private EndSimulationEntityCommandBufferSystem endSimulationECBSystem;

    // private bool startValueSet = false;

    protected override void OnCreate()
    {
        // endSimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        // Initialize our player data
        player = EntityManager.CreateEntity();

        resources = new Dictionary<ResourceTypes, Text>();  // UI textfield mapping

        DynamicBuffer<PlayerResourceData> playerResources = EntityManager.AddBuffer<PlayerResourceData>(player);

        #region Initialize player resources (buffer)
        
        playerResources.Add(new PlayerResourceData { 
            resource = new ResourceData {
                type = ResourceTypes.Gold,
                value = startResourceValue
            }
         });

        playerResources.Add(new PlayerResourceData { 
            resource = new ResourceData {
                type = ResourceTypes.Wood,
                value = startResourceValue
            }
         });

        playerResources.Add(new PlayerResourceData { 
            resource = new ResourceData {
                type = ResourceTypes.Iron,
                value = startResourceValue
            }
         });

        playerResources.Add(new PlayerResourceData { 
            resource = new ResourceData {
                type = ResourceTypes.Crystal,
                value = startResourceValue
            }
         });

        #endregion       

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

        // if(startValueSet)
            // return;

        // startValueSet = true;
    }

    protected override void OnUpdate()
    {
        // To-do: Refactor
        // Without any usage of ecb atm...
        // var parallelECB = endSimulationECBSystem.CreateCommandBuffer().AsParallelWriter();

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

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            Entities
                    .WithoutBurst()
                    .ForEach(
                        (Entity entity, in ResourceTransactionData transactionData) =>
                        {
                            if(!transactionData.isProfit)
                                SpendResource(transactionData.type, (int) transactionData.value);
                            else
                                AddResource(transactionData.type, (int) transactionData.value);
                            
                            ecb.DestroyEntity(entity);
                        }
                    ).Run();


            // endSimulationECBSystem.AddJobHandleForProducer(this.Dependency);
    }

    private bool SpendResource(ResourceTypes type, int val)
    {
        bool state = false;

        // To-do: Refactor

        Entities
                .ForEach(
                    (ref DynamicBuffer<PlayerResourceData> buffer) =>
                    {
                        for(int i = 0; i < buffer.Length; i++)
                        {
                            if(buffer[i].resource.type == type) // Type search 
                            {
                                if(buffer[i].resource.value >= val) // Validate spending
                                {
                                    Debug.Log($"Spending {buffer[i].resource.value} of {buffer[i].resource.type}.");
                                    
                                    buffer[i] = new PlayerResourceData 
                                    {
                                        resource = new ResourceData
                                        {
                                            type = buffer[i].resource.type, 
                                            value = buffer[i].resource.value - val
                                        }
                                    };

                                    state = true;
                                    break;
                                }
                            }
                        }

                        // Exit from ForEach? We don't need something else atm                       
                        return;
                    }
                ).Run();

        return state;
    }

    private bool AddResource(ResourceTypes type, int val)
    {
        return SpendResource(type, -val);
    }

    private bool ValidateSpending(int current, int val)
    {
        if(val > current)
            return false;
        
        return true;
    }

    private void SetResourceText(ResourceTypes type, int val)
    {
        resources[type].text = val.ToString();
    }
}
