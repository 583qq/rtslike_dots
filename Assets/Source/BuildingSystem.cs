using System;

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Rendering;

using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingSystem : SystemBase
{
    private Entity prefabSelectedNotify;
    private ConstructionTask task;

    private void GetConstructBuildingEntity()
    {
        // Building to Construct Tag => Reference Component
        EntityQuery notifyQuery = GetEntityQuery(typeof(ConstructionTaskNotify));

        if(notifyQuery.IsEmpty)
        {
            this.Enabled = false;
            return;
        }

        prefabSelectedNotify = notifyQuery.ToEntityArray(Allocator.Temp)[0];

        var notify = EntityManager.GetComponentData<ConstructionTaskNotify>(prefabSelectedNotify).task;

        // Validation
        if(!EntityManager.Exists(notify))
            return;

        task = EntityManager.GetComponentData<ConstructionTask>(notify);

        EntityManager.DestroyEntity(notify);
    }

    protected override void OnStartRunning()
    {
        GetConstructBuildingEntity();
    }

    protected override void OnStopRunning()
    {
        if(EntityManager.Exists(prefabSelectedNotify))
            EntityManager.DestroyEntity(prefabSelectedNotify);
    }

    private void RenderPreview()
    {

    }

    protected override void OnUpdate()
    {
        Vector3 position = GameUtilities.MouseToTerrainPosition();

        RenderPreview();

        if(Mouse.current.leftButton.IsPressed()) // LMouseClick
        {
            this.Enabled = false;
            Build(position);
            return;
        }

        if(Mouse.current.rightButton.IsPressed()) // RMouseClick
        {
            this.Enabled = false;
            return;
        }
    }
    
    // Building on mouse position
    private void Build(Vector3 position)
    {   
        // Rewrite all the shit EntityManager => EntityCommandBuffer (if it could be executed not on the same frame/thread)
        // This is structural change, so it's somewhat slow if we are building a lot

        Entity buildingPrefab = task.buildingPrefab;

        bool attackable = task.isAttackable;
        uint durability = task.durability;
        uint constructionTime = task.constructionTime;

        // Our new building entity
        Entity buildingEntity = EntityManager.Instantiate(buildingPrefab);
        
        // Cause it's not prefab now
        EntityManager.RemoveComponent<PrefabTag>(buildingEntity);

        // Set position
        EntityManager.SetComponentData<Translation>(buildingEntity, new Translation {
            Value = (float3) position
        });

        //  Add BuildingTag & SelectableTag
        EntityManager.AddComponent<BuildingTag>(buildingEntity);
        EntityManager.AddComponent<SelectableTag>(buildingEntity);

        if(attackable)
            SetAttackable(buildingEntity, durability);

        //  Add BuildingConstructionComponent (inConstruction)
        EntityManager.AddComponentData<BuildingConstructionComponent>(buildingEntity, new BuildingConstructionComponent {
            workToComplete = constructionTime
        });
    }

    /*
        TO-DO:
        Refactor some Attackable logic there...
        And move it to another system/file (migrate into unit system?)
    */

    // Use when entity is not attackable
    private void SetAttackable(Entity unit, uint _durability)
    {
        // If it's already Attackable just return from function
        if(isAttackable(unit))
            return;

        EntityManager.AddComponentData<AttackableComponent>(unit, new AttackableComponent {
            durability = _durability,
            current = _durability
        });
    }

    // Use when entity is attackable
    private void SetDurability(Entity unit, uint _durability)
    {
        if(!isAttackable(unit))
            return;
        
        // Remark: Just set some component data, without getting component data

        AttackableComponent _component = EntityManager.GetComponentData<AttackableComponent>(unit);
        _component.durability = _durability;

        SetAttackableComponent(unit, _component);
    }

    // Reset current to maximum durability
    private void ResetDurability(Entity unit)
    {
        if(!isAttackable(unit))
            return;
        
        // Remark: Just set some component data, without getting component data?

        AttackableComponent _component = GetAttackableComponent(unit);
        _component.current = _component.durability;

        SetAttackableComponent(unit, _component);
    }

    // Take Damage
    private void Damage(Entity unit, uint damage)
    {
        if(!isAttackable(unit))
            return;

        AttackableComponent _component = GetAttackableComponent(unit);
       
        uint durability_points;

        // Validate & Set
        if(_component.current < damage)
            durability_points = 0;
        else
            durability_points = _component.current - damage;
        //

        _component.current = durability_points;

        SetAttackableComponent(unit, _component);
    }

    // Heal
    private void Repair(Entity unit, uint heal)
    {
        if(!isAttackable(unit))
            return;

        AttackableComponent _component = GetAttackableComponent(unit);

        uint durability_points;

        // if it's greater than uint.MaxValue than just SET IT to the maximum
        if(GameUtilities.CheckUintSumOverflow(_component.current, heal))
            durability_points = _component.durability;
        else
            durability_points = _component.current + heal; // Else Set As It Is.

        // Validate & Set
        if(durability_points > _component.durability)
            _component.current = _component.durability;
        else
            _component.current = durability_points;
        //

        SetAttackableComponent(unit, _component);
    }

    private bool isAttackable(Entity entity) => EntityManager.HasComponent<AttackableComponent>(entity);
    private AttackableComponent GetAttackableComponent(Entity entity) => EntityManager.GetComponentData<AttackableComponent>(entity);
    private void SetAttackableComponent(Entity entity, AttackableComponent c) => EntityManager.SetComponentData<AttackableComponent>(entity, c);
}
