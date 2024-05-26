using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringPBDManager : MonoBehaviour
{
    [Header("Particles attributes")]
    public float[] mass;
    public float[] inverseMass;
    public Vector3[] velocities;
    public GameObject[] particles;
    public Vector3[] positions;
    public int nbParticles = 2;


    [Header("Spring attributes")]
    public float l0 = 0.5f;

    [Header("Simulation attributes")]
    public float dt = 1/60f;
    Vector3 gravity = new Vector3(0, -9.81f, 0);
    public GameObject particlePrefab;
    public float k = 0;
    public float alpha; 


    [Header("Moving bounds")]
    public float minX = -5f;
    public float maxX = 5f;
    public float minY = -5f;
    public float maxY = 5f;
    public float minZ = -5f;
    public float maxZ = 5f;

    // Start is called before the first frame update
    void Start()
    {
        mass = new float[nbParticles];
        inverseMass = new float[nbParticles];
        mass[0] = 0.5f;
        mass[1] = 0.5f;
        inverseMass[0] = 1 / mass[0];
        inverseMass[1] = 1 / mass[1];

        velocities = new Vector3[nbParticles];
        positions = new Vector3[nbParticles];

        // Instantiate our 2 particles participating in the spring
        particles[0] = Instantiate(particlePrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
        particles[1] = Instantiate(particlePrefab, new Vector3(0, 0, 0.3f), Quaternion.identity, transform);

        alpha = 1/k;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int numsteps = 100;
        float sdt = dt / numsteps;
        for (int p = 0; p < numsteps; p++) {
            // 
            for (int i = 0; i < nbParticles; i++) {
                velocities[i] += gravity * sdt;
                positions[i] = particles[i].transform.position;
                particles[i].transform.position += sdt * velocities[i];
            }

            // Calculate lambda used in the correction vector
            float diff = Vector3.Distance(particles[1].transform.position, particles[0].transform.position); 
            float lambda = CalculateLambda(diff, sdt);

            // Solve constraints
            for (int j = 1; j < nbParticles; j++) {
                // Spring constraint
                Vector3 grad = particles[j].transform.position - particles[j - 1].transform.position;
                Vector3 delta0 = lambda * inverseMass[j - 1] * grad.normalized;
                Vector3 delta1 = lambda * inverseMass[j] * grad.normalized;
                Debug.Log("Delta0 : " + delta0);
                Debug.Log("Delta1 : " + delta1);

                //particles[j - 1].transform.position -= delta0;
                particles[0].transform.position = new Vector3(0, 0, 0);
                particles[j].transform.position += delta1;

            }

            // Update velocities
            for (int k = 0; k < nbParticles; k++) {
                velocities[k] = (particles[k].transform.position - positions[k]) / sdt;
            }
        }
    }

    public float CalculateLambda(float l, float dt) {

        float lambda = -(l - l0)/ (inverseMass[0] + inverseMass[1]);

        return lambda;
    }
}

