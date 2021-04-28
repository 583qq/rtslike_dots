using System;
using System.Collections.Generic;

using Unity;
using Unity.Entities;

using UnityEngine;

public interface ISkillImplementation
{
    void Cast(Entity entity);
}

public interface ISkillEffect 
{
    SkillEffectTarget target_type { get; set; }
}

public interface ISkillCost 
{
    SkillCost cost {get; set;}
}

public enum SkillEffectTarget
{
    Target,
    Area,
    Self
}

public enum SkillState
{
    Passive,
    Active,
    Cooldown,
    Casting
}

public struct SkillCost
{

}

public struct SkillTime
{
    public readonly float origin;
    public float current;

    public SkillTime(float val)
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

public struct SkillRange
{

}

public struct UnitSkill
{
    public SkillState state {get; set;}
    public SkillTime cooldown;
    public SkillTime castTime;
    public SkillRange range;
    public bool inRange;
    public bool areResourcesEnough;

}

[System.Serializable]
public struct BasicAttackProjectile : ISkillEffect
{
    public SkillEffectTarget target_type {get; set; }

    public void Cast(Entity entity)
    {

    }
}