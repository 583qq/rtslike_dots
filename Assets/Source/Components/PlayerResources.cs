using Unity;
using Unity.Entities;

public enum ResourceTypes
{
    None,
    Gold,
    Wood,
    Iron,
    Crystal   
}

[InternalBufferCapacity(4)]
public struct PlayerResourceData : IBufferElementData
{
    public ResourceData resource;
}

public struct ResourceData
{
    public ResourceTypes type;
    public int value;
}