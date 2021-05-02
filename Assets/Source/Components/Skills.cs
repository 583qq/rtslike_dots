using System;
using System.Collections.Generic;

using Unity;
using Unity.Entities;

using UnityEngine;


public struct AbilityEffectBuffer : IBufferElementData
{
    public AbilityEffect effect;
}

public struct AbilityEffect : ISharedComponentData
{
    public AbilityTargetType targetType;
    public float baseDamageMinimum;
    public float baseDamageMaximum;
    
    // Animation
}

public enum AbilityTargetType
{
    Target,
    Area,
    Self
}

public enum AbilityState
{
    Passive,
    Active,
    Cooldown,
    Casting
}

public enum DamageType
{
    Physical,
    Fire,
    Frost,
    Arcane,
    Chaos
}

public enum AbilityCostType
{
    Mana,
    Energy
}

public struct AbilityCost
{
    public AbilityCostType type;
    public int value;
}

public struct AbilityTime
{
    public readonly float origin;
    public float current;

    public AbilityTime(float val)
    {
        origin = val;
        current = origin;
    }

    public bool isElapsed()
    {
        return current <= 0;
    }

    public void Update(float delta)
    {
        current -= delta;
    }

    public void Reset()
    {
        current = origin;
    }
}

public struct UnitAbility
{
    public AbilityState state {get; set;}
    public AbilityTime cooldown;
    public AbilityTime castTime;
    public Range range;
    public bool inRange;
    public bool areResourcesEnough;

}
