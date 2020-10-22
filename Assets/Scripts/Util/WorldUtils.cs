using UnityEngine;
using System.Collections;
using LA2RTS.LA2Entities;

public class WorldUtils
{

    //each Unity unit equals 16 L2 units
    public static float WORLD_SCALE_COEFFICIENT = 16.0f;

    public static Vector3 L2ToUnityCoords(int x, int y, int z)
    {
        return new Vector3(-x / WORLD_SCALE_COEFFICIENT, z / WORLD_SCALE_COEFFICIENT, y / WORLD_SCALE_COEFFICIENT);
    }

    public static Vector3 UnityToL2Coords(Vector3 vec)
    {
        return new Vector3(-vec.x * WORLD_SCALE_COEFFICIENT, vec.z * WORLD_SCALE_COEFFICIENT, vec.y * WORLD_SCALE_COEFFICIENT);
    }

    public static Quaternion GetRotationToFacePos(Vector3 ojectPos, Vector3 targetPos)
    {
        //find the vector pointing from our position to the target
        var _direction = (targetPos - ojectPos).normalized;

        //create the rotation we need to be in to look at the target
        var _lookRotation = Quaternion.LookRotation(_direction);

        return _lookRotation;
    }

    public static Quaternion ActorDefaultRotation()
    {
        return Quaternion.Euler(0, 0, 0);
    }

    public static float CalculateDistance(LA2Spawn t1, LA2Spawn t2)
    {
        Vector3 v1 = new Vector3(t1.X, t1.Y, t1.Z);
        Vector3 v2 = new Vector3(t2.X, t2.Y, t2.Z);
        return (v1 - v2).magnitude;
    }

    public static float CalculateDistance(LA2Spawn t1, Vector3 t2)
    {
        Vector3 v1 = new Vector3(t1.X, t1.Y, 0);
        return (v1 - t2).magnitude;
    }

    public static float CalculateDistance(Vector3 t1, Vector3 t2)
    {
        return (t1 - t2).magnitude;
    }

}
