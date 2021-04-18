using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MapBoundariesData : IComponentData
{
    public float MaxBoundaryX;
    public float MaxBoundaryZ;
    public float MinBoundaryX;
    public float MinBoundaryZ;
}
