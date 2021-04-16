using Unity;
using Unity.Entities;


// Buffer?

[GenerateAuthoringComponent]
public struct SelectedBuilding : IComponentData
{
    Entity selected;
}