using Unity;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Jobs;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class UnitSelectionSystem : SystemBase
{
    private bool anySelected = false;
    private bool mousePressed;
    private bool mouseReleased;
    private Vector2 screenStartPosition;
    private Vector2 screenEndPosition;

    private float selectionDistanceMinimum = 2.0f;
    private float selectionDistanceMinimumSqr;

    private float3 bottomLeft;
    private float3 topRight;

    private RectTransform boxTransform;

    private Camera view;
    private Mouse mouse;
    private EventSystem eventSystem;

    private EndSimulationEntityCommandBufferSystem endSimulationECBSystem;

    protected override void OnCreate()
    {
        endSimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
        selectionDistanceMinimumSqr = selectionDistanceMinimum * selectionDistanceMinimum;

        var boxQuery = GetEntityQuery(typeof(SelectionBoxTag), typeof(RectTransform));

        if(boxQuery.IsEmpty)
        {
            Debug.Log("Can't find entity with SelectionBoxTag component.");
            return;
        }

        var boxEntity = boxQuery.ToEntityArray(Allocator.Temp)[0];

        boxTransform = EntityManager.GetComponentObject<RectTransform>(boxEntity);

        view = Camera.main;
        mouse = Mouse.current;
        eventSystem = EventSystem.current;
    }

    protected override void OnStopRunning()
    {
    }

    protected override void OnUpdate()
    {
        var ecb = endSimulationECBSystem.CreateCommandBuffer().AsParallelWriter();

        // If you are over UI elements you can't select
        if(eventSystem.IsPointerOverGameObject())
            return;
        
        // UI SelectionArea
        if(mouse.leftButton.IsPressed())
        {
            UpdateBoxSelectionArea();
        }

        if(mouse.leftButton.wasReleasedThisFrame)
            mouseReleased = true;

        // Selection Logic
        if(mouseReleased)
        {
            boxTransform.sizeDelta = new Vector2(0, 0);
            mousePressed = false;
            mouseReleased = false;

            #region Click-like selection
            // Vector created with two points
            Vector3 distVector = (Vector3) bottomLeft - (Vector3) topRight;

            float distance = distVector.magnitude;

            if(distance <= selectionDistanceMinimum)
            {
                bottomLeft += new float3(-1, 0, -1) * (selectionDistanceMinimum - distance) / 2;
                topRight += new float3(1, 0, 1) * (selectionDistanceMinimum - distance) / 2;
            }
            #endregion


            Debug.Log($"Trying to select: {bottomLeft} => {topRight}");
            HandleUnitSelection(bottomLeft, topRight, ecb);
        }

        // Deselection Logic
        if(anySelected && mouse.rightButton.IsPressed())
            DeselectUnits(ecb);


        endSimulationECBSystem.AddJobHandleForProducer(this.Dependency);
    }

    private void DeselectUnits(EntityCommandBuffer.ParallelWriter ecb)
    {
        anySelected = false;

        // Deselection Logic
        Entities
                .ForEach(
                    (Entity entity, int entityInQueryIndex, in UnitSelectedTag tag) =>
                    {
                        ecb.RemoveComponent<UnitSelectedTag>(entityInQueryIndex, entity);
                    }
                ).ScheduleParallel();
    }

    private void UpdateBoxSelectionArea()
    {
        if(!mousePressed)
        {
            screenStartPosition = mouse.position.ReadValue();
            mousePressed = true;
        }

        screenEndPosition = mouse.position.ReadValue();

        // World space position floats
        float3 startPosition = (float3) view.ScreenToWorldPoint(new Vector3(screenStartPosition.x, 
                                                 screenStartPosition.y, view.transform.position.y));
        float3 endPosition =  (float3) view.ScreenToWorldPoint(new Vector3(screenEndPosition.x, 
                                                 screenEndPosition.y, view.transform.position.y));

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
        boxTransform.sizeDelta = new Vector2(screenMaxAnchor.x - screenMinAnchor.x, 
                                             screenMaxAnchor.y - screenMinAnchor.y);
    }

    private void HandleUnitSelection(float3 minAnchor, float3 maxAnchor, EntityCommandBuffer.ParallelWriter ecb)
    {
        Entities
                .WithAll<SelectableTag>()
                .ForEach(
                    (Entity entity, int entityInQueryIndex, in Translation translation) =>
                    {
                        float3 position = translation.Value;

                        if(position.x >= minAnchor.x &&
                           position.z >= minAnchor.z &&
                           position.x <= maxAnchor.x &&
                           position.z <= maxAnchor.z)
                           {                               
                               Debug.Log($"Selected: {entity} ({position})");
        
                               ecb.AddComponent<UnitSelectedTag>(entityInQueryIndex, entity);
                           }
                    }).ScheduleParallel(); 
    }

}