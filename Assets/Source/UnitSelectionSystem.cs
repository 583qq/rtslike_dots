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
    private Vector2 screenStartPosition;
    private Vector2 screenEndPosition;

    private float3 bottomLeft;
    private float3 topRight;

    private RectTransform boxTransform;

    private Camera view;


    protected override void OnStartRunning()
    {
        var boxQuery = GetEntityQuery(typeof(SelectionBoxTag), typeof(RectTransform));

        if(boxQuery.IsEmpty)
        {
            Debug.Log("Can't find entity with SelectionBoxTag component.");
            return;
        }


        var boxEntity = boxQuery.ToEntityArray(Allocator.Temp)[0];

        boxTransform = EntityManager.GetComponentObject<RectTransform>(boxEntity);

        Debug.Log($"Selection Rect: {boxTransform.rect}");

        view = Camera.main;
    }

    protected override void OnStopRunning()
    {
    }

    protected override void OnUpdate()
    {
        if(Mouse.current.leftButton.IsPressed())
        {
            UpdateBoxSelectionArea();
        }

        if(Mouse.current.leftButton.wasReleasedThisFrame)
            mouseReleased = true;

        if(mouseReleased)
        {
            boxTransform.sizeDelta = new Vector2(0, 0);
            mousePressed = false;
            mouseReleased = false;

            // Check if distance is small
            // Set our default rectangle

            Debug.Log($"Trying to select: {bottomLeft} => {topRight}");
            HandleUnitSelection(bottomLeft, topRight);
        }
    }

    private void UpdateBoxSelectionArea()
    {
        if(!mousePressed)
        {
            screenStartPosition = Mouse.current.position.ReadValue();
            mousePressed = true;
        }

        screenEndPosition = Mouse.current.position.ReadValue();

        // World space position floats
        float3 startPosition = (float3) view.ScreenToWorldPoint(new Vector3(screenStartPosition.x, screenStartPosition.y, view.transform.position.y));
        float3 endPosition =  (float3) view.ScreenToWorldPoint(new Vector3(screenEndPosition.x, screenEndPosition.y, view.transform.position.y));

        // Box Transform 'anchors'
        Vector2 screenMinAnchor = new Vector2(math.min(screenStartPosition.x, screenEndPosition.x), 
                                              math.min(screenStartPosition.y, screenEndPosition.y));

        Vector2 screenMaxAnchor = new Vector2(math.max(screenStartPosition.x, screenEndPosition.x),
                                              math.max(screenStartPosition.y, screenEndPosition.y));

        // World positions
        bottomLeft = new float3(math.min(startPosition.x, endPosition.x), 0, 
                                math.min(startPosition.z, endPosition.z));

        topRight = new float3(math.max(startPosition.x, endPosition.x), 0,
                              math.max(startPosition.z, endPosition.z));


        boxTransform.position = screenMinAnchor;
        boxTransform.sizeDelta = new Vector2(screenMaxAnchor.x - screenMinAnchor.x, screenMaxAnchor.y - screenMinAnchor.y);
    }

    private void HandleUnitSelection(float3 bottom_left, float3 top_right)
    {
        Entities
                .WithAll<SelectableTag>()
                .ForEach(
                    (Entity entity, ref Translation translation) =>
                    {
                        float3 position = translation.Value;

                        if(position.x >= bottom_left.x &&
                           position.z >= bottom_left.z &&
                           position.x <= top_right.x &&
                           position.z <= top_right.z)
                           {
                               Debug.Log($"Selected: {entity} ({position})");
                               // Add selected entities into native array?
                               // Or add selected component?
                           }
                    }
                ).ScheduleParallel();
    }

}