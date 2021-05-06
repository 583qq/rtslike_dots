using System;
using System.Collections.Generic;

using Unity;
using Unity.Entities;

using UnityEngine;


public struct AbilityEffectBuffer : IBufferElementData
{
    public AbilityEffect effect;
}

public struct AbilityUsed : IComponentData
{
    public AbilityEffect effect;
}

[Serializable]
public struct AbilityEffect
{
    public AbilityEffectType effectType;
    public bool isHealing;
    public float baseValueMin;
    public float baseValueMax;
    
    // Animation
    // > Cast/Apply/Use
    // > Mid Point (projectile)
    // > Impact 
}

public enum AbilityEffectType
{
    Target,
    Projectile,
    ProjectileNonTarget,
    Area,
    Self
}

public struct AbilityTarget : IComponentData
{
    public Entity target;
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

public struct AbilityCost
{
    public UnitEnergyType type;
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
}
