using Unity;
using Unity.Entities;
using UnityEngine;

public enum UnitEnergyType
{
    Mana,
    Energy
}

public struct UnitEnergyComponent : IComponentData
{
    public UnitEnergyType type;
    public int origin;
    public int value;
}

public struct AttackableComponent : IComponentData
{
    public uint durability;
    public uint current;
}