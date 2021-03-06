using Unity;

using UnityEngine;
using UnityEngine.InputSystem;

class GameUtilities : MonoBehaviour
{
    public static bool CheckUintSumOverflow(uint x, uint y)
    {
        long value = 0;

        value = (long) x + (long) y;

        if(value > uint.MaxValue)
            return true;

        return false;
    }

    public static bool CheckUintMultiplyOverflow(uint x, uint y)
    {
        ulong value = 0;

        // value maximum = (2^32 - 1)^2,  ulong maximum 2^64 - 1
        // (2^32 - 1)(2^32 - 1) < 2^64 - 1  => True
        
        value = (ulong) x * (ulong) y;

        if(value > uint.MaxValue)
            return true;
        
        return false;
    }


    public static Vector3 MouseToTerrainPosition()
    {
        Vector3 position = Vector3.zero;

        // Ignore UI (Layer 5)
        int layerMask = 1 << 5;
        layerMask = ~layerMask;

        if(Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit info, 500, layerMask))
            position = info.point;

        return position;
    }

    public static Vector3 ScreenPositionToTerrainPosition(Vector2 screen_pos)
    {
        Vector3 position = Vector3.zero;
        
        int layerMask = 1 << 5;
        layerMask = ~layerMask;

        if(Physics.Raycast(Camera.main.ScreenPointToRay(screen_pos), out RaycastHit info, 500, layerMask))
            position = info.point;

        return position;
    }
}