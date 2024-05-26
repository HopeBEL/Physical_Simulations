using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bead : MonoBehaviour
{
    Vector3 velocity;
    Vector3 previousPosition;
    public GameObject wire;
    CircleCollider2D wireCollider;
    Vector3 gravity = new Vector3(0, -9.81f, 0);
    float dt = 1/60f;
    int numSteps = 10;

    // Rigidity
    public float k = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // We use a collider only to access the center and radius of the circle
        wireCollider = wire.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StartStep(dt);
        KeepOnWireConstraint();
        EndStep(dt);
        
    }

    // Apply gravity forces and predict position accordingly
    void StartStep(float dt) {
        velocity += gravity * dt;
        previousPosition = transform.position;
        transform.position += velocity * dt;
    }

    // Resolve the constraint, which is that the bead must always stay on the wire
    void KeepOnWireConstraint() {
        // Gradient calculation
        Vector3 direction = transform.position - (Vector3)wireCollider.offset;
        float length = direction.magnitude;
        if (length < 0.0001f) return;

        // Gradient normalize to only have the direction
        direction = direction.normalized;

        // Constraint error lambda calculation
        float lambda = wireCollider.radius - length;

        // Apply the error correction
        transform.position += direction * lambda;
    }

    // Update velocities
    void EndStep(float dt) {
        velocity = (transform.position - previousPosition);
        velocity = velocity/dt;
    }
}
