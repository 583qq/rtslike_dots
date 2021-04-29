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
    
    protected override void OnUpdate()
    {
        for(int i = 0; i < gridSize; i+=2)
        {
            Debug.DrawLine(new Vector3(i, gridHeight, 0), new Vector3(i, gridHeight, gridSize), Color.green);
            Debug.DrawLine(new Vector3(0, gridHeight, i), new Vector3(gridSize, gridHeight, i), Color.green);
        }   
    }
}
}