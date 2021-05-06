using Unity;
using Unity.Entities;

using UnityEngine;


namespace Game { 
public class UnitAbilitySystem : SystemBase
{
    UnitSystem unitSystem;
    EndSimulationEntityCommandBufferSystem endSimulationECBSystem;

    protected override void OnCreate()
    {
        unitSystem = World.GetExistingSystem<UnitSystem>();
        endSimulationECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        ApplyTargetAbilities();
    }

    private void ApplyTargetAbilities()
    {
        Entities
                .WithoutBurst()
                .ForEach(
                    (in AbilityUsed ability, in AbilityTarget targetComponent) =>
                    {
                        if(ability.effect.effectType == AbilityEffectType.Target)
                        {
                            float average = (ability.effect.baseValueMin + ability.effect.baseValueMax) / 2;

                            if(ability.effect.isHealing)
                            {
                                // I'm thinking about just pushing notify-like entity for this. So we can do it parallel
                                unitSystem.Repair(targetComponent.target, (uint) average);
                                return;
                            }

                            unitSystem.Damage(targetComponent.target, (uint) average);
                        }
                    }
                ).Run();     
    }

    // Main-thread first iteration
    public void SetTarget(Entity caster, AbilityTarget target)
    {
        EntityManager.AddComponentData(caster, target);
    }

}

}