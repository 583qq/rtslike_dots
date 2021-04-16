using Unity;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

using UnityEngine;
using UnityEngine.InputSystem;

public struct BoxSelectArea : IComponentData
{
    public float x, y; // point
    public float width, height;
}

public class UnitSelectionSystem : SystemBase
{
    private bool mousePressed;
    private bool mouseReleased;
    private Vector2 boxStartPosition;
    private Vector2 boxEndPosition;
    Entity selectedArea;
    
    private Material boxMaterial;
    private RenderMesh boxMesh;

    private const float boxHeight = 2.0f;

    public int uiLayer = 5;

    private Canvas canvas;

    protected override void OnCreate()
    {
        var canvasQuery = GetEntityQuery(typeof(Canvas));

        if(canvasQuery.IsEmpty)
            return;

        var canvasEntity = canvasQuery.ToEntityArray(Allocator.Temp)[0];

        canvas = EntityManager.GetComponentObject<Canvas>(canvasEntity);

        Debug.Log($"CanvasObject: {canvas}");
    }

    protected override void OnStartRunning()
    {
        selectedArea = EntityManager.CreateEntity();
    }

    protected override void OnStopRunning()
    {
        EntityManager.DestroyEntity(selectedArea);
    }

    protected override void OnUpdate()
    {
        if(Mouse.current.leftButton.IsPressed())
        {
            // Something is not quite right
            // UpdateBoxSelectionAreaEntity();
        }
        else
        {
            mousePressed = false;
        }

        if(mouseReleased)
            HandleUnitSelection();
    }

    private void UpdateBoxSelectionAreaEntity()
    {
        if(!mousePressed)
        {
            boxStartPosition = Mouse.current.position.ReadValue();
            mousePressed = true;
        }

        // Create new entity with:
        // Rect Transform
        // Canvas Renderer
        // Image?

        boxEndPosition = Mouse.current.position.ReadValue();

        Vector3 worldStartPosition = GameUtilities.ScreenPositionToTerrainPosition(boxStartPosition);
        Vector3 worldEndPosition = GameUtilities.ScreenPositionToTerrainPosition(boxEndPosition);

        UnityEngine.Debug.Log($"[Selection] | Screen: {boxStartPosition} > {boxEndPosition}");
        UnityEngine.Debug.Log($"[Selection] | World: {worldStartPosition} > {worldEndPosition}");

        // World coordinates
        float _width = worldEndPosition.x - worldStartPosition.x;
        float _height = worldEndPosition.y - worldStartPosition.y;

        // I don't need it for UI
        EntityManager.AddComponentData<Translation>(selectedArea, new Translation {
            Value = (float3) (new Vector3(worldStartPosition.x, boxHeight, worldStartPosition.y))
        });

        
        // Debug
        EntityManager.SetComponentData<BoxSelectArea>(selectedArea, new BoxSelectArea {
            x = worldStartPosition.x,
            y = worldStartPosition.y,
            width = _width,
            height = _height 
        });
    }

    private void HandleUnitSelection()
    {

    }

}