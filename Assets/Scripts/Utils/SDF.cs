using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDF
{
    static Vector3 Vec3Abs(Vector3 p) {
        return new Vector3(Mathf.Abs(p.x), Mathf.Abs(p.y), Mathf.Abs(p.z));
    }

    public static float box(Vector3 p, Vector3 b) // center, size
    {
        Vector3 q = Vec3Abs(p) - b;
        return Vector3.Magnitude(new Vector3(Mathf.Max(q.x, 0f), Mathf.Max(q.y, 0f), Mathf.Max(q.z, 0f) )) + Mathf.Min(Mathf.Max(q.x,Mathf.Max(q.y,q.z)), 0f);
    }
}
