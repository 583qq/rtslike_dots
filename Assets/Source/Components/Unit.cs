using Unity;
using Unity.Entities;

public struct PriceData : IComponentData
{
    public ResourceTypes ResourceType;
    public uint Price;
}