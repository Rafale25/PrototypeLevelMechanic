using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Random.insideUnitCircle.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += velocity * Time.deltaTime * 1f;
        this.transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
    }
}
