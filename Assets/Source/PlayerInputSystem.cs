using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;
using UnityEngine.InputSystem;

//*******************************
// 
// Camera System (Input? Haha)
// 'Input' isn't right...
// 
//*******************************

public class PlayerInputSystem : SystemBase
{
    private InputControls controls;

    private Vector2 camMovementDirection;
    private float camMovementSpeed = 10.0f;

    private float camFOVdelta;
    private float camFOVspeed = 2.0f;
    private float camFOVminimum = 20.0f;
    private float camFOVmaximum = 100.0f;

    private bool isCameraMoving = false;

    private Camera playerView = null;

    private MapBoundariesData boundaries;

    private float boundThickness = 5.0f;


    protected override void OnCreate()
    {
        if(controls == null)
        {
            controls = new InputControls();

            controls.Camera.Movement.performed += ctx =>
            {
                camMovementDirection = ctx.ReadValue<Vector2>();
                isCameraMoving = camMovementDirection.x != 0 || camMovementDirection.y != 0;
            };

            controls.Camera.Movement.canceled += ctx =>
            {
                isCameraMoving = false;
            };

            controls.Camera.FOV.performed += ctx =>
            {
                camFOVdelta = ctx.ReadValue<float>();
                ChangeFOV();
            };
        }
    }

    protected override void OnStartRunning()
    {
        controls.Camera.Enable();

        InitializePlayerView();

        Debug.Log($"[Camera Initial Position]: {playerView.transform.position}");
    }  

    protected override void OnStopRunning()
    {
        controls.Camera.Disable();
    }

    protected override void OnUpdate()
    {
        if(isCameraMoving)
        {
            MoveCamera();
        }

        if(MouseInBounds())
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            
            // Raycast on UI layer if hit
            // return;

            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            
            camMovementDirection = (mousePosition - screenCenter).normalized;

            MoveCamera();
        }
    }

    // ...
    private void LogNotFound(string components)
    {
        Debug.Log($"[ERROR]: NO ENTITIES WITH {components} COMPONENT DETECTED.");
    }

    private void InitializePlayerView()
    {
        #region PlayerView Camera
        
        var playerViewQuery = GetEntityQuery(typeof(PlayerViewTag), typeof(Camera));

        if(playerViewQuery.IsEmpty)
        {
            LogNotFound("Camera, PlayerViewTag");
            return;
        }

        var entitiesWithPlayerView = playerViewQuery.ToEntityArray(Allocator.Temp);
        
        var playerViewEntity = entitiesWithPlayerView[0]; // Get first (anyway it's only one atm)
        
        #endregion

        #region Map Boundaries

        var boundariesQuery = GetEntityQuery(typeof(MapBoundariesData));

        if(boundariesQuery.IsEmpty)
        {
            LogNotFound("MapBoundariesData");
            return;
        }

        var mapBoundaries = boundariesQuery.ToEntityArray(Allocator.Temp)[0];

        #endregion
        
        boundaries = EntityManager.GetComponentData<MapBoundariesData>(mapBoundaries);

        playerView = EntityManager.GetComponentObject<Camera>(playerViewEntity);
    }

    private bool MouseInBounds()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector2 gameResolution = new Vector2(Screen.width, Screen.height);

        // Fix it & Refactor
        // To-do: check if our mouse/pointer position is out of game screen

        if(mousePosition.x < boundThickness || 
           mousePosition.y < boundThickness ||
           mousePosition.x > gameResolution.x - boundThickness ||
           mousePosition.y > gameResolution.y - boundThickness)
            return true;

        return false;
    }

    void MoveCamera()
    {
        if(playerView == null)
            return;

        float dT = UnityEngine.Time.deltaTime;

        Vector3 direction = new Vector3(camMovementDirection.x, 0, camMovementDirection.y);

        Vector3 position = playerView.transform.position + direction * camMovementSpeed * dT;

        // Map Boundaries

        if(position.x > boundaries.MaxBoundaryX)
            position.x = boundaries.MaxBoundaryX;
        if(position.x < boundaries.MinBoundaryX)
            position.x = boundaries.MinBoundaryX;

        if(position.z > boundaries.MaxBoundaryZ)
            position.z = boundaries.MaxBoundaryZ;
        if(position.z < boundaries.MinBoundaryZ)
            position.z = boundaries.MinBoundaryZ;

        playerView.transform.position = position;
    }

    void ChangeFOV()
    {
        if(playerView == null)
            return;
        
        float dT = UnityEngine.Time.deltaTime;

        playerView.fieldOfView -= camFOVdelta * camFOVspeed * dT;

        // FOV Boundaries
        if(playerView.fieldOfView > camFOVmaximum)
            playerView.fieldOfView = camFOVmaximum;
        if(playerView.fieldOfView < camFOVminimum)
            playerView.fieldOfView = camFOVminimum; 
    }


}
