using Unity;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

using UnityEngine;


public class SelectedDrawSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
                .WithAll<UnitSelectedTag>()
                .ForEach(
                    (Entity entity, in Translation translation) =>
                    {
                        
                    }
                ).Run();
    }
}