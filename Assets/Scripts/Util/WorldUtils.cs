using UnityEngine;
using System.Collections;

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

}
