using System;

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
public class BuildingSystem : SystemBase
{
    private Entity prefabSelectedNotify;
    private ConstructionTask task;

    private Mesh previewMesh;
    private Material previewMaterial;

    private Material previewMaterialGreen;
    private Material previewMaterialRed;

    private GridSystem gridSystem;
    private PlayerResourceSystem playerResourceSystem;

    private UnitPriceData[] prices;

    protected override void OnCreate()
    {
        gridSystem = World.GetExistingSystem<GridSystem>();
        playerResourceSystem = World.GetExistingSystem<PlayerResourceSystem>();
    }

    public void SetBuildingPrice(UnitPriceData[] prices)
    {
        this.prices = prices;
    }

    private void SpendResources()
    {
        foreach(var price in prices)
        {
            bool validationStatus = playerResourceSystem.ValidateResourceSpending(price.type, (int) price.value);

            if(!validationStatus)
                return;

            playerResourceSystem.SpendResource(price.type, (int) price.value);  
        }
    }

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

    protected override void OnStopRunning()
    {
        if(EntityManager.Exists(prefabSelectedNotify))
            EntityManager.DestroyEntity(prefabSelectedNotify);
    }

    protected override void OnStartRunning()
    {
        GetConstructBuildingEntity();

        // Preview Mesh Scale
        RenderMesh rmesh = EntityManager.GetSharedComponentData<RenderMesh>(task.previewPrefab);

        previewMesh = rmesh.mesh;
        previewMaterial = rmesh.material;

        // Can place
        previewMaterialGreen = new Material(previewMaterial);
        previewMaterialGreen.SetColor("_BaseColor", Color.green);   // _Color without Lit?

        // Can't place
        previewMaterialRed = new Material(previewMaterial);
        previewMaterialRed.SetColor("_BaseColor", Color.red);
    }

    private void RenderPreview(Vector3 position, bool canPlace)
    {
        Vector3 scale = new Vector3(3, 3, 3);
        Matrix4x4 trsMatrix = Matrix4x4.TRS(position, Quaternion.identity, scale);
        
        if(canPlace)
            Graphics.DrawMesh(previewMesh, trsMatrix, previewMaterialGreen, 0);
        else
            Graphics.DrawMesh(previewMesh, trsMatrix, previewMaterialRed, 0);
    }

    protected override void OnUpdate()
    {
        Vector3 position = GameUtilities.MouseToTerrainPosition();

        Vector2 gridPosition = gridSystem.WorldToGrid(position);

        bool isEmpty = gridSystem.IsEmpty((int) gridPosition.x, (int) gridPosition.y);

        // Debug.Log($"{isEmpty} : {gridPosition}");

        Vector3 gridWorldPosition = gridSystem.CellToWorld(gridPosition);

        // Debug.Log($"P: {position} => {gridWorldPosition}");

        RenderPreview(gridWorldPosition, isEmpty);

        if(Mouse.current.leftButton.IsPressed()) // LMouseClick
        {
            if(!isEmpty)
            {
                Debug.Log("Cell is not empty!");
                return;
            }

            this.Enabled = false;
            Build(gridWorldPosition, gridPosition);
            return;
        }

        // To-do: Validate that both buttons aren't pressed...
        // if so, do nothing or cancel

        if(Mouse.current.rightButton.IsPressed()) // RMouseClick
        {
            this.Enabled = false;
            return;
        }
    }
    
    // Building on mouse position
    private void Build(Vector3 position, Vector2 gridPosition)
    {   
        SpendResources();

        gridSystem.Fill((int) gridPosition.x, (int) gridPosition.y, Cell.Filled);

        // Rewrite all the shit EntityManager => EntityCommandBuffer (if it could be executed not on the same frame/thread)
        // This is structural change, so it's somewhat slow if we are building a lot?

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
    private void SetDurability(Entity unit, uint durability)
    {
        if(!isAttackable(unit))
            return;
        
        // Remark: Just set some component data, without getting component data

        AttackableComponent component = EntityManager.GetComponentData<AttackableComponent>(unit);
        component.durability = durability;

        SetAttackableComponent(unit, component);
    }

    // Reset current to maximum durability
    private void ResetDurability(Entity unit)
    {
        if(!isAttackable(unit))
            return;
        
        // Remark: Just set some component data, without getting component data?

        AttackableComponent component = GetAttackableComponent(unit);
        component.current = component.durability;

        SetAttackableComponent(unit, component);
    }

    // Take Damage
    private void Damage(Entity unit, uint damage)
    {
        if(!isAttackable(unit))
            return;

        AttackableComponent component = GetAttackableComponent(unit);
       
        uint durability_points;

        // Validate & Set
        if(component.current < damage)
            durability_points = 0;
        else
            durability_points = component.current - damage;
        //

        component.current = durability_points;

        SetAttackableComponent(unit, component);
    }

    // Heal
    private void Repair(Entity unit, uint heal)
    {
        if(!isAttackable(unit))
            return;

        AttackableComponent component = GetAttackableComponent(unit);

        uint durabilityPoints;

        // if it's greater than uint.MaxValue than just SET IT to the maximum
        if(GameUtilities.CheckUintSumOverflow(component.current, heal))
            durabilityPoints = component.durability;
        else
            durabilityPoints = component.current + heal; // Else Set As It Is.

        // Validate & Set
        if(durabilityPoints > component.durability)
            component.current = component.durability;
        else
            component.current = durabilityPoints;
        //

        SetAttackableComponent(unit, component);
    }

    private bool isAttackable(Entity entity) => EntityManager.HasComponent<AttackableComponent>(entity);
    private AttackableComponent GetAttackableComponent(Entity entity) => EntityManager.GetComponentData<AttackableComponent>(entity);
    private void SetAttackableComponent(Entity entity, AttackableComponent c) => EntityManager.SetComponentData<AttackableComponent>(entity, c);
}

}