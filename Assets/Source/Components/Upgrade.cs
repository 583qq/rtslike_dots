using Unity;
using Unity.Entities;



// Price for building Upgrade
public struct UpgradePriceBuffer : IBufferElementData
{
    ResourceData price;
}


// Upgraded entities prefabs
public struct UpgradeBuildingBuffer : IBufferElementData
{
    Entity upgradedBuilding;
}