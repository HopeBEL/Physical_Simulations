using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleLinked : MonoBehaviour
{
    Vector3 velocity;
    Vector3 previousPosition;
    public GameObject boulder;
    CircleCollider2D boulderCollider;
    Vector3 gravity = new Vector3(0, -9.81f, 0);
    float dt = 1/60f;
    int numSteps = 10;
    public float k = 1f;
    public GameObject[] circleLinked;

    /*public struct CircleObject {
        public GameObject circle;
        public float mass;
    }*/

    // Start is called before the first frame update
    void Start()
    {
        boulderCollider = boulder.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float sdt = dt / numSteps;
        for (int i = 0; i < numSteps; i++) {
            StartStep(sdt);
            KeepOnWireConstraint();
            EndStep(sdt);
        }
        
    }

    void StartStep(float dt) {
        velocity += gravity * dt;
        previousPosition = transform.position;
        transform.position += velocity * dt;
    }

    void KeepOnWireConstraint() {
        Vector3 direction = transform.position - (Vector3)boulderCollider.offset;
        float length = direction.magnitude;
        if (length < 0.0001f) return;
        // On normalise le vecteur
        direction = direction.normalized;

        // Calcul de constraint error
        float lambda = boulderCollider.radius - length;
        transform.position += k * direction * lambda;
    }

    void EndStep(float dt) {
        velocity = (transform.position - previousPosition);
        velocity = velocity/dt;
    }
}
