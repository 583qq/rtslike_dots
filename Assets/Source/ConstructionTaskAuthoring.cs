using Unity;
using Unity.Entities;

using System.Collections.Generic;

using UnityEngine;


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
    private Entity taskEntity;

    [SerializeField] private UnitPriceData[] prices;

    private bool isConverted = false;

    void Start()
    {
        world = World.DefaultGameObjectInjectionWorld;
        manager = world.EntityManager; 
    }

    // Should be converted one time in the GameObjectConversionGroup
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Debug.Log($"Converting Entity: {entity}");

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
        Debug.Log($"Added referenced prefabs: {buildingPrefab} & {previewPrefab}");
    }
    
    public void Construction()
    {
        if(!isConverted)
            return;

        
        foreach(var price in prices)
        {
            var price_entity = manager.CreateEntity();
            manager.AddComponentData<EntityReference>(price_entity, new EntityReference
            {
                reference = taskEntity
            });

            manager.AddComponentData<PriceData>(price_entity, new PriceData
            {
                ResourceType = price.type,
                Price = price.price
            });
        }

        Debug.Log($"Construction Task Notify: {taskEntity}");

        var notify = manager.CreateEntity(typeof(ConstructionTaskNotify));
        manager.AddComponentData<ConstructionTaskNotify>(notify, new ConstructionTaskNotify
        { task = taskEntity });
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
    public uint price;
}

public struct EntityReference : IComponentData
{
    public Entity reference;
}