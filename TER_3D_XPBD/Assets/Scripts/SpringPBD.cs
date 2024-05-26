using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringPBD : MonoBehaviour
{
    public float mass = 5f;
    public float inverseMass = 0f;
    public Vector3 gradient;
    public Vector3 velocities;

    // Rest distance of the spring
    public float l0 = 5f;
    public GameObject[] particles;
    public int nbParticles = 2;
    Vector3 gravity = new Vector3(0, -9.81f, 0);
    public Vector3 positions;
    public float dt = 1/60f;
    public int particleIndex;

    // Start is called before the first frame update
    void Start()
    {
        inverseMass = 1 / mass;
        //velocities = new Vector3[nbParticles];
        //gradient = new Vector3[nbParticles];
        //positions = new Vector3[nbParticles];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocities += gravity * dt;
        positions = transform.position;
        transform.position += dt * velocities;

        float diff = Vector3.Distance(particles[1].transform.position, particles[0].transform.position); 
        float lambda = CalculateLambda(diff);

        // Solve spring constraint
        if (particleIndex == 0) 
            gradient = (particles[1].transform.position - particles[0].transform.position).normalized;
        else if (particleIndex == 1) 
            gradient = - (particles[1].transform.position - particles[0].transform.position).normalized;

        Vector3 delta = CalculateCorrection(lambda, gradient);
        transform.position += 0.5f * delta;
        particles[1 - particleIndex].transform.position -= 0.5f * delta; // Appliquer la correction à l'autre particule
        velocities = (transform.position - positions);
        velocities = velocities / dt;
    }

    public float CalculateLambda(float l) {

        float lambda = -(l - l0) / (inverseMass * 1 + inverseMass * 1);

        return lambda;
    }

    public Vector3 CalculateCorrection(float lambda, Vector3 gradient) {
        Vector3 delta = new Vector3(0, 0, 0);

        delta = lambda * inverseMass * gradient;
        return delta;
    }


}
