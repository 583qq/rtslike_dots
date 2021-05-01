using Unity;
using Unity.Entities;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


namespace Game 
{
[DisallowMultipleComponent]
[AddComponentMenu("Gameplay Feature/Unit/Buildings/Construct Building Task")]
[ConverterVersion("NoWayRage", 1)]
public class ConstructionTaskAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    // Component Data
    public GameObject buildingPrefab;
    public GameObject previewPrefab;
    public bool isAttackable;
    [Range(0, 300)]
    public uint constructionTime;
    public uint durability;


    // Authoring Script
    private World world;
    private EntityManager manager;
    private PlayerResourceSystem playerResourceSystem;
    private BuildingSystem buildingSystem;

    private Entity taskEntity;

    [SerializeField] private UnitPriceData[] prices;

    private bool isConverted = false;

    void Start()
    {
        world = World.DefaultGameObjectInjectionWorld;
        manager = world.EntityManager; 
        
        buildingSystem = world.GetExistingSystem<BuildingSystem>();
        playerResourceSystem = world.GetExistingSystem<PlayerResourceSystem>();
    }

    void Awake()
    {
        // if(playerResourceSystem == null)
        //     Debug.Log("PlayerResourceSystem is null!");

        // if(buildingSystem == null)
        //     Debug.Log("BuildingSystem is null!");
    }

    // Should be converted one time in the GameObjectConversionGroup
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        ConstructionTask _task = default(ConstructionTask);
        _task.buildingPrefab = conversionSystem.GetPrimaryEntity(buildingPrefab);
        _task.previewPrefab = conversionSystem.GetPrimaryEntity(previewPrefab);
        _task.isAttackable = isAttackable;
        _task.constructionTime = constructionTime;
        _task.durability = durability;

        dstManager.AddComponentData(entity, _task);

        isConverted = true;

        taskEntity = entity;
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(buildingPrefab);
        referencedPrefabs.Add(previewPrefab);
    }
    
    public void Construction()
    {
        if(!isConverted)
            return;

        // Refactor this & validate resource spending (send array)
        foreach(var price in prices)
        {
            bool validationStatus = playerResourceSystem.ValidateResourceSpending(price.type, (int) price.value);

            if(!validationStatus)
            {
                Debug.Log($"Not enough {price.type}!");
                return;
            }
        }

        buildingSystem.SetBuildingPrice(prices);

        Debug.Log($"Construction Task Notify: {taskEntity}");

        var notify = manager.CreateEntity(typeof(ConstructionTaskNotify));
        manager.AddComponentData<ConstructionTaskNotify>(notify, new ConstructionTaskNotify
        { 
            task = taskEntity 
        });
    }
}

public struct ConstructionTask : IComponentData
{
    public Entity buildingPrefab;
    public Entity previewPrefab;
    public bool isAttackable;
    public uint constructionTime;
    public uint durability;
}

[System.Serializable]
public struct UnitPriceData 
{
    public ResourceTypes type;
    public uint value;
}

public struct EntityReference : IComponentData
{
    public Entity reference;
}


}