using Unity;
using Unity.Entities;
using UnityEngine;


[GenerateAuthoringComponent]
public struct PriceDataTag : IComponentData
{
    public ResourceTypes ResourceType;
}