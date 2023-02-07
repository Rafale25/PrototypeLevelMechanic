using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [Range(0f, 3f)]
    public float separationFactor = 0.59f;

    [Range(0f, 3f)]
    public float alignmentFactor = 1.87f;

    [Range(0f, 3f)]
    public float cohesionFactor = 1.34f;

    [Range(0f, 5f)]
    public float viewDistance = 3f;

    [Range(0f, 50f)]
    public float arenaRadius = 6f;
    
    public Boid boidPrefab;
    private List<Boid> boids;
    private int boidCount = 80;
    private float speed = 5f;
    private float spawnRadius = 5f;
    
 
    void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, arenaRadius);
    }

    void Start()
    {
        boids = new List<Boid>();

        for (int i = 0 ; i < boidCount ; ++i) {
            Boid boid = Instantiate(boidPrefab, transform);
            boid.transform.position += Random.insideUnitSphere * spawnRadius;
            boids.Add(boid);
        }
    }

    void FixedUpdate()
    {
        foreach (Boid me in boids) {
            Vector3 newPosition = me.transform.position;
            Vector3 newVelocity = me.velocity;

            int nb_neighbours = 0;
            Vector3 averageDirection = Vector3.zero;
            Vector3 averagePosition = Vector3.zero;
            Vector3 totalForce = Vector3.zero;

            foreach (Boid other in boids) {
                if (me == other) {
                    continue;
                }

                float dist = Vector3.SqrMagnitude(newPosition - other.transform.position);

                if (dist < viewDistance * viewDistance) {
                    nb_neighbours += 1;
                    averageDirection += other.velocity;
                    averagePosition += other.transform.position;
                    totalForce += (newPosition - other.transform.position) / dist;
                }

            }

            // separation
            newVelocity += totalForce * separationFactor;

            if (nb_neighbours != 0) {
                // alignment
                averageDirection /= nb_neighbours;
                newVelocity += averageDirection * alignmentFactor;

                // cohesion
                averagePosition /= nb_neighbours;
                newVelocity += -(newPosition - averagePosition) * cohesionFactor;
            }

            // distance limit
            float df = (newPosition - this.transform.position).magnitude - arenaRadius;
            if (df > 0f) {
                Vector3 to_center = (this.transform.position - newPosition).normalized;
                newVelocity += to_center * (df * 0.1f);
            }

            me.transform.position = newPosition;
            newVelocity *= 1.01f;
            me.velocity = Vector3.ClampMagnitude(newVelocity, speed);
        }
    }
}
