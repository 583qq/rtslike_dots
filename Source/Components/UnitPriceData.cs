using Unity;
using Unity.Entities;
using UnityEngine;

public enum ResourceTypes
{
    Gold,
    Wood,
    Stone,
    Iron   
}

[GenerateAuthoringComponent]
public struct UnitPriceData : IComponentData
{
    public ResourceTypes ResourceType;
    public uint Price;
}