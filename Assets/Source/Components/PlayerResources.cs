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


public struct PlayerResourceData : IBufferElementData
{
    public ResourceData resource;
}

public struct ResourceData
{
    public ResourceTypes type;
    public int value;
}