using System;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

using Unity.Rendering;


[GenerateAuthoringComponent]
public struct ConstructionTaskNotify : IComponentData
{
    public Entity task;
}


public struct BuildingTag : IComponentData
{
}


// Construction System =>
public struct BuildingConstructionComponent : IComponentData
{
    public uint workToComplete;
    public uint workCurrent;
    public bool isDone;
}
// when isDone => Delete comopnent from entity ?