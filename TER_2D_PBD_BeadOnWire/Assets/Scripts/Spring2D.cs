using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringPBDManager : MonoBehaviour
{
    public float[] mass;
    public float[] inverseMass;
    public Vector3[] gradient;
    public Vector3[] velocities;

    // Rest distance of the spring
    public float l0 = 0.2f;
    public GameObject[] particles;
    public int nbParticles = 2;
    Vector3 gravity = new Vector3(0, -9.81f, 0);
    public Vector3[] positions;
    public float dt = 1/60f;
    public GameObject particlePrefab;
    public float k = 0;
    public float alpha; 
    // Start is called before the first frame update
    void Start()
    {
        mass = new float[nbParticles];
        inverseMass = new float[nbParticles];
        mass[0] = 1f;
        mass[1] = 0.5f;
        inverseMass[0] = 1 / mass[0];
        inverseMass[1] = 1 / mass[1];
        velocities = new Vector3[nbParticles];
        gradient = new Vector3[nbParticles];
        positions = new Vector3[nbParticles];

        //particles[0] = Instantiate(particlePrefab, new Vector3(Random.Range(-6f, -3f), Random.Range(3f, 6f), Random.Range(-6f, -3f)), Quaternion.identity, transform);
        particles[0] = Instantiate(particlePrefab, new Vector3(-10, 0, 10), Quaternion.identity, transform);
        particles[1] = Instantiate(particlePrefab, new Vector3(10, 2, -10), Quaternion.identity, transform);
        //particles[1] = Instantiate(particlePrefab, new Vector3(Random.Range(3f, 6f), Random.Range(3f, 6f), Random.Range(3f, 6f)), Quaternion.identity, transform);
        alpha = 1/k;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int numsteps = 100;
        float sdt = dt / numsteps;
        for (int p = 0; p < numsteps; p++) {
            for (int i = 0; i < nbParticles; i++) {
                velocities[i] += gravity * sdt;
                positions[i] = particles[i].transform.position;
                particles[i].transform.position += sdt * velocities[i];
            }

            float diff = Vector3.Distance(particles[1].transform.position, particles[0].transform.position); 
            
            //float diff = Mathf.Pow(Mathf.Pow(particles[1].transform.position.x - particles[0].transform.position.x, 2) + Mathf.Pow(particles[1].transform.position.y - particles[0].transform.position.y, 2) + Mathf.Pow(particles[1].transform.position.z - particles[0].transform.position.z, 2) ,0.5f);
            float lambda = CalculateLambda(diff, sdt);

            // Solve spring constraint
            gradient[0] = (particles[0].transform.position - particles[1].transform.position).normalized;
            //gradient[1] = (particles[1].transform.position - particles[0].transform.position).normalized;
            gradient[1] = -gradient[0];
            
            for (int j = 1; j < nbParticles; j++) {
                Debug.Log("Je suis là");
                //Vector3 delta = CalculateCorrection(j, lambda);
                Vector3 grad = particles[j].transform.position - particles[j - 1].transform.position;
                Vector3 delta0 = lambda * inverseMass[j - 1] * grad.normalized;
                Vector3 delta1 = lambda * inverseMass[j] * grad.normalized;
                particles[j - 1].transform.position -= delta0;
                particles[j].transform.position += delta1;
            }

            for (int k = 0; k < nbParticles; k++) {
                velocities[k] = (particles[k].transform.position - positions[k]) / sdt;
                //velocities[k] = velocities[k] / sdt;
            }
        }
    }

    public float CalculateLambda(float l, float dt) {

        float lambda = -(l - l0) / l / (inverseMass[0] + inverseMass[1]);

        return lambda;
    }

    public Vector3 CalculateCorrection(int i, float lambda) {
        Vector3 delta = lambda * inverseMass[i] * gradient[i];

        return delta;
    }


}

