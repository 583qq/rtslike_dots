using System;

using Unity;
using Unity.Entities;

using UnityEngine;


namespace Game
{
public class DebugGridDrawSystem : SystemBase
{
    private int gridSize = 128;
    private float gridHeight = 0f;
    
    private const int gridCellSize = 2;

    private Color gridColor = Color.green;
    
    protected override void OnUpdate()
    {
        for(int i = 0; i < gridSize; i+=gridCellSize)
        {
            Debug.DrawLine(new Vector3(i, gridHeight, 0), new Vector3(i, gridHeight, gridSize), gridColor);
            Debug.DrawLine(new Vector3(0, gridHeight, i), new Vector3(gridSize, gridHeight, i), gridColor);
        }   
    }
}
}