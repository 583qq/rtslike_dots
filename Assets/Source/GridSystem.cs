using System;

using Unity;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;

using UnityEngine;


public enum Cell
{
    Empty,
    Filled,
    Blocked
}


public class GridSystem : SystemBase
{
    // Grid
    public const int width = 64;
    public const int height = 64;
    // World
    const int terrain_width = 128;
    const int terrain_height = 128;

    // 
    public bool IsEmpty(int x, int y) => (grid[x * width + y] == Cell.Empty) ? true : false;
    public bool IsFilled(int x, int y) =>  (grid[x * width + y] != Cell.Empty) ? true : false; 
    public void Fill(int x, int y, Cell val) => grid[x * width + y] = val;

    // Should I use simple arrays?
    NativeArray<Cell> grid;

    protected override void OnCreate()
    {
        grid = new NativeArray<Cell>(width * height, Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        grid.Dispose();
    }

    protected override void OnUpdate()
    {
    }

    public Vector2 WorldToGrid(Vector3 worldPosition)
    {
        int tx = (int) (worldPosition.x / (terrain_width / width)); 
        int tz = (int) (worldPosition.z / (terrain_height / height));

        return new Vector2(tx, tz);
    }

    public Vector3 CellToWorld(Vector2 cell)
    {
        float cellCenterX = (terrain_width / width) / 2;
        float cellCenterY = (terrain_height / height) / 2;
        
        float tx = cell.x * (terrain_width / width) + cellCenterX;
        float ty = cell.y * (terrain_height / height) + cellCenterY;

        return new Vector3(tx, 0.0f, ty);
    }
}
