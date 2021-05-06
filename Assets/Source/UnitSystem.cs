using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;


namespace Game
{
public class UnitSystem : SystemBase
{
    protected override void OnStartRunning()
    {

    }
    protected override void OnUpdate()
    {
        //
    }

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
    public void Damage(Entity unit, uint damage)
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
    public void Repair(Entity unit, uint heal)
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