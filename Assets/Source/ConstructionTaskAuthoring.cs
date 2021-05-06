using Unity;
using Unity.Collections;
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
    // public GameObject buildingPrefab;
    // public GameObject previewPrefab;
    // public bool isAttackable;
    // [Range(0, 300)]
    // public uint constructionTime;
    // public uint durability;

    public ConstructionTaskObject building;


    // Authoring Script
    private World world;
    private EntityManager manager;
    private PlayerResourceSystem playerResourceSystem;
    private BuildingSystem buildingSystem;

    private Entity taskEntity;

    private UnitPriceData[] prices;

    [SerializeField] private UnitTaskObject[] units;

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
        ConstructionTask task = default(ConstructionTask);
        task.buildingPrefab = conversionSystem.GetPrimaryEntity(building.buildingPrefab);
        task.previewPrefab = conversionSystem.GetPrimaryEntity(building.previewPrefab);
        task.isAttackable = building.isAttackable;
        task.constructionTime = building.constructionTime;
        task.durability = building.durability;

        prices = building.prices;

        dstManager.AddComponentData(entity, task);

        isConverted = true;

        taskEntity = entity;
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(building.buildingPrefab);
        referencedPrefabs.Add(building.previewPrefab);
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
                buildingSystem.Enabled = false;
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


public struct SpawnableUnitAsset
{
    public BlobString name;
    public Entity prefab;
    // Tags
    public bool isAttackable;
    public bool isHero;
    public bool isRanged;
    //
    public uint spawnTime;
    // Stats
    public float health;
    public DamageType damageType;
    public Range baseDamage;
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