using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision {
    bool CheckSphereExtra(Collider target_collider, SphereCollider sphere_collider, out Vector3 closest_point, out Vector3 surface_normal)
    {
        closest_point = Vector3.zero;
        surface_normal = Vector3.zero;
        float surface_penetration_depth = 0f;

        Vector3 sphere_pos = sphere_collider.transform.position;
        
        if (Physics.ComputePenetration(target_collider,target_collider.transform.position,target_collider.transform.rotation,sphere_collider, sphere_pos, Quaternion.identity,out surface_normal,out surface_penetration_depth))
        {
            closest_point = sphere_pos + (surface_normal * (sphere_collider.radius - surface_penetration_depth));
            surface_normal = -surface_normal;

            return true;
        }
        return false;
    }

}
