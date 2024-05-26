using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticuleBtwWall : MonoBehaviour
{
    [Header("Particles attributes")]
    public float[] mass;
    public float[] inverseMass;
    public Vector3[] velocities;
    public GameObject[] particles;
    public Vector3[] positions;
    public int nbParticles = 3;
    float k = 0.5f;
    float alpha;

    [Header("Spring attributes")]
    public float l0 = 0.3f;

    [Header("Simulation attributes")]
    public float dt = 1/60f;
    Vector3 gravity = new Vector3(0, -9.81f, 0);
    public GameObject particlePrefab;

    // Start is called before the first frame update
    void Start()
    {
        mass = new float[nbParticles];
        inverseMass = new float[nbParticles];
        mass[0] = 0.5f;
        mass[1] = 0.5f;
        mass[2] = 0.5f;
        inverseMass[0] = 0;
        inverseMass[1] = 0;
        inverseMass[2] = 1 / mass[2];

        velocities = new Vector3[nbParticles];
        positions = new Vector3[nbParticles];
        particles[0] = Instantiate(particlePrefab, new Vector3(-1f, 0, 0), Quaternion.identity, transform);
        particles[1] = Instantiate(particlePrefab, new Vector3(1f, 0, 0), Quaternion.identity, transform);
        particles[2] = Instantiate(particlePrefab, new Vector3(0.3f, 0, 0), Quaternion.identity, transform);

        alpha = 1 / k;
    }

      // Update is called once per frame
    void FixedUpdate()
    {
        int numsteps = 100;
        float sdt = dt / numsteps;
        for (int p = 0; p < numsteps; p++) { // à bouger
            for (int i = 0; i < nbParticles; i++) {
                //velocities[i] += gravity * sdt;
                positions[i] = particles[i].transform.position;
                particles[i].transform.position += sdt * velocities[i];
            }

            // Calculate lambda used in the correction vector
            float diffLeft = Vector3.Distance(particles[2].transform.position, particles[0].transform.position); 
            float diffRight = Vector3.Distance(particles[2].transform.position, particles[1].transform.position); 
            
            float lambdaLeft = CalculateLambda(diffLeft, sdt, 2);
            float lambdaRight = CalculateLambda(diffRight, sdt, 2);

            // Solve constraints
            // Spring constraint
            Vector3 gradLeft = particles[2].transform.position - particles[0].transform.position;
            Vector3 gradRight = particles[2].transform.position - particles[1].transform.position;
            //Vector3 deltaLeft = lambdaLeft * inverseMass[2] * (diffLeft > l0 ?  -gradLeft.normalized : gradLeft.normalized);
            Vector3 deltaLeft = lambdaLeft * inverseMass[2] * gradLeft.normalized;
            //Vector3 deltaRight = lambdaRight * inverseMass[2] * (diffRight > l0 ? gradRight.normalized : -gradRight.normalized);
            Vector3 deltaRight = lambdaRight * inverseMass[2] * gradRight.normalized;

            particles[0].transform.position = new Vector3(-1f, 0, 0);
            particles[1].transform.position = new Vector3(1f, 0, 0);
            particles[2].transform.position -= deltaLeft;
            particles[2].transform.position += deltaRight;

            // Update velocities
            for (int k = 0; k < nbParticles; k++) {
                velocities[k] = (particles[k].transform.position - positions[k]) / sdt;
            }

            Debug.Log("sdt: " + sdt);
            Debug.Log("lambdaLeft: " + lambdaLeft);
            Debug.Log("lambdaRight: " + lambdaRight);
            Debug.Log("deltaLeft: " + deltaLeft);
            Debug.Log("deltaRight: " + deltaRight);
        }
    }

    public float CalculateLambda(float l, float dt, int i) {

        float lambda = -(l - l0)/ (inverseMass[i] + alpha/(dt*dt));

        return lambda;
    }
}

