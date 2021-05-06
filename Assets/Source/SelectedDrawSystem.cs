using Unity;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

using UnityEngine;


public class SelectedDrawSystem : SystemBase
{
    Mesh selectionMesh;
    Material selectionMaterial;

    protected override void OnCreate()
    {
        // Get Mesh & Material
    }

    protected override void OnUpdate()
    {
        var mesh = selectionMesh;
        var material = selectionMaterial;

        // Should I draw IT like this?
        // Should I add shared render selection mesh?

        Entities
                .WithAll<UnitSelectedTag>()
                .WithoutBurst()
                .ForEach(
                    (Entity entity, in Translation translation) =>
                    {
                        Graphics.DrawMesh(
                        mesh,
                        translation.Value,
                        Quaternion.identity,
                        material,
                        0);
                    }
                ).Run();
    }
}