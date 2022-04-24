using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vezie
{
    public static Vector3 Vezie_3(Vector3 start_pos, Vector3 center_pos, Vector3 end_pos, float ratio)
    {
        Vector3 p1 = start_pos * (1.0f - ratio) + center_pos * (ratio);
        Vector3 p2 = center_pos * (1.0f - ratio) + end_pos * (ratio);
        Vector3 returnPos = p1 * (1.0f - ratio) + p2 * (ratio);

        return returnPos;
    }
}