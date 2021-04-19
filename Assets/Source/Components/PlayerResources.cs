using Unity;
using Unity.Entities;

public enum ResourceTypes
{
    Gold,
    Wood,
    Iron,
    Crystal   
}

public struct PlayerData : IComponentData
{
    public int Gold;
    public int Wood;
    public int Iron;
    public int Crystal;
}