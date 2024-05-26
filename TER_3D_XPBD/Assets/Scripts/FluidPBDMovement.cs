using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidPBDMovement : MonoBehaviour
{
    /* Contraintes :
        - distance : ne pas rentrer dans une autre particule
        - densité
        - pression
    */

    public GameObject[] particles;
    public Vector3[] velocities;
    public Vector3[] gradient;
    public Vector3[] correction;
    public Vector3[] newPos;
    public Vector3[] previousPos;
    public float dt = 1/60f;
    int numSteps = 10;
    public int nbParticles = 20;
    public GameObject fluidParticleObject;
    private Vector3 gravity = new Vector3(0, -9.81f, 0);

    //public float h = 1f;
    public float radius = 0.9f;
    public float poly6Kernel;
    public float poly6Coeff = 0f;
    public float[] density;
    public float spikyCoeff = 0f;
    public float rho0 = 1000f;
    public List<int>[] neighborsArray;
    public float minX = -1f;
    public float maxX = 1f;
    public float minY = 0f;
    public float maxY = 4f;
    public float minZ = -1f;
    public float maxZ = 1f;

    // Start is called before the first frame update
    void Start()
    {
        particles = new GameObject[nbParticles];
        correction = new Vector3[nbParticles];
        gradient = new Vector3[nbParticles];
        velocities = new Vector3[nbParticles];
        newPos = new Vector3[nbParticles];
        previousPos = new Vector3[nbParticles];
        for (int i = 0; i < nbParticles; i++) {
            particles[i] = Instantiate(fluidParticleObject, transform.position + new Vector3(Random.Range(-0.1f, 0.1f), 2f, Random.Range(-0.1f, 0.1f)), Quaternion.identity, gameObject.transform);
        }
        density = new float[nbParticles];
        poly6Coeff = 315 / (64 * Mathf.PI * Mathf.Pow(radius, 9));
        spikyCoeff = 15 / (Mathf.PI * Mathf.Pow(radius, 6));
        neighborsArray = new List<int>[nbParticles];
    }

    // Update is called once per frame
    void Update()
    {
        //float sdt = dt / numSteps;
        //Debug.Log(sdt);
        int iter = 0;
        int solverIterations = 10;
        for (int i = 0; i < nbParticles; i++) {
            // Apply gravity
            velocities[i] += gravity * dt;
            // Predict position
            newPos[i] = particles[i].transform.position + velocities[i] * dt;
        }
        for (int i = 0; i < nbParticles; i++) {
            // Find neighbours of particle i
            List<int> neighborsList = FindNeighbors(i);
            neighborsArray[i] = neighborsList;
        }
        while (iter < solverIterations) {
            iter++;
            for (int i = 0; i < nbParticles; i++) {
                // Calculate lambda
                CalculateDensity(i);
                CalculateGradient(i);
                CalculateLambda(i);
            }

            for (int i = 0; i < nbParticles; i++) {
                CalculateCorrection(i);

                        // Collision detection between particles
                for (int j = 0; j < neighborsArray[i].Count; j++) {
                    Debug.Log("lààààà");
                    float distanceCenters = Vector3.Distance(particles[i].GetComponent<SphereCollider>().center, particles[j].GetComponent<SphereCollider>().center);
                    float radius1 = particles[i].GetComponent<SphereCollider>().radius;
                    float radius2 = particles[j].GetComponent<SphereCollider>().radius;
                    if (distanceCenters - (radius1 + radius2) < 0) {
                        // faire quelque chose
                        Debug.Log("Collision détectée");
                        Vector3 collisionDirection = (particles[i].transform.position - particles[j].transform.position).normalized;
                        float penetrationDistance = (radius1 + radius2) - distanceCenters;
                        particles[i].transform.position -= collisionDirection * penetrationDistance * 0.5f;
                        particles[j].transform.position += collisionDirection * penetrationDistance * 0.5f;
                    }
                }

                // If we go underground
                if (particles[i].transform.position.y < 0f) {
                    Vector3 collisionDirection = (particles[i].transform.position - new Vector3(0, 0, 0)).normalized;
                    float penetrationDistance = -particles[i].transform.position.y;
                    //particles[i].transform.position += collisionDirection * penetrationDistance * 0.5f;
                    velocities[i] += new Vector3(0, penetrationDistance * 0.5f, 0);
                    particles[i].transform.position = new Vector3(particles[i].transform.position.x, 0f, particles[i].transform.position.z);
                }
            }

            // Update positions
            for (int k = 0; k < nbParticles; k++) {
                previousPos[k] = newPos[k];
                newPos[k] += correction[k];
                /*if (newPos[k].x > maxX) {
                    newPos[k].x = maxX;
                } else if (newPos[k].x < minX) {
                    newPos[k].x = minX;
                }
                if (newPos[k].y > maxY) {
                    newPos[k].y = maxY;
                } else if (newPos[k].y < minY) {
                    newPos[k].y = minY;
                }
                if (newPos[k].z > maxZ) {
                    newPos[k].z = maxZ;
                } else if (newPos[k].z < minZ) {
                    newPos[k].z = minZ;
                */
            }
        }

        // PROBLEME ICI
        for (int k = 0; k < nbParticles; k++) {
            //Debug.Log(correction[k]);
            
            // Update velocities
            velocities[k] = (newPos[k] - particles[k].transform.position) / dt;
            
            // Apply vorticity confinement and XSPH viscosity

            // Update previous positions
            particles[k].transform.position = newPos[k];
        }
    }

    List<int> FindNeighbors(int index) {
        List<int> neighborsList = new List<int>();
        for (int i = 0; i < nbParticles; i++) {
            if (Vector3.Distance(newPos[index], newPos[i]) < radius) {
                if (i != index)
                    neighborsList.Add(i);
            }
        }

        return neighborsList;
    }

    public void ResolveConstraint(int index) {
        // Calculate Lambda
        CalculateDensity(index);
        CalculateGradient(index);
        CalculateLambda(index);
        CalculateCorrection(index);
        

        
        // Collision detection between particles
        for (int i = 0; i < neighborsArray[index].Count; i++) {
            Debug.Log("lààààà");
            float distanceCenters = Vector3.Distance(particles[index].GetComponent<SphereCollider>().center, particles[i].GetComponent<SphereCollider>().center);
            float radius1 = particles[index].GetComponent<SphereCollider>().radius;
            float radius2 = particles[i].GetComponent<SphereCollider>().radius;
            if (distanceCenters - (radius1 + radius2) < 0) {
                // faire quelque chose
                Debug.Log("Collision détectée");
                Vector3 collisionDirection = (particles[index].transform.position - particles[i].transform.position).normalized;
                float penetrationDistance = (radius1 + radius2) - distanceCenters;
                particles[index].transform.position -= collisionDirection * penetrationDistance * 0.5f;
                particles[i].transform.position += collisionDirection * penetrationDistance * 0.5f;
            }
        }
    }

    public float Poly6(float r) {
        float h = radius;
        if (r <= h) {
            return Mathf.Pow((Mathf.Pow(h, 2) - Mathf.Pow(r, 2)),3) * poly6Coeff;
        } 
        else {
            return 0;
        }
    }

    public float Spiky(float r) {
        float h = radius;
        if (r <= h) {
            return Mathf.Pow((h - r),3) * spikyCoeff;
        }
        else {
            return 0;
        }
    }

    public void CalculateDensity(int i) {
        for (int j = 0; j < neighborsArray[i].Count; j++)
            density[i] += Poly6(Vector3.Distance(particles[i].transform.position, particles[neighborsArray[i][j]].transform.position));
    }

    public void CalculateGradient(int i) {
        float spiky = 0f;
        for (int k = 0; k < nbParticles; k++) {
            if (k == i) {
                for (int j = 0; j < neighborsArray[i].Count; j++)
                {
                    spiky = Spiky(Vector3.Distance(particles[i].transform.position, particles[neighborsArray[i][j]].transform.position));
                    gradient[i] += new Vector3(spiky, spiky, spiky);
                }
            }
            else {
                spiky = Spiky(Vector3.Distance(particles[i].transform.position, particles[k].transform.position));
                gradient[i] -= new Vector3(spiky, spiky, spiky);

            }
        }
        gradient[i] *= 1 / rho0;
    }

    public float CalculateLambda(int i){
        float lambda = 0;
        float sumGradientsNormSqr = 0;
        if (neighborsArray[i] != null) {
            for (int k = 0; k < neighborsArray[i].Count; k++) {
                sumGradientsNormSqr += gradient[neighborsArray[i][k]].sqrMagnitude;
            }
            lambda = -(density[i] / rho0 - 1) / sumGradientsNormSqr;
        }
        
        return lambda;
    }

    public void CalculateCorrection(int i) {
        float scal = 0;
        if (neighborsArray[i] != null)
            for (int j = 0; j < neighborsArray[i].Count; j++) {
                scal = (CalculateLambda(i) + CalculateLambda(neighborsArray[i][j])) * Spiky(Vector3.Distance(particles[i].transform.position, particles[neighborsArray[i][j]].transform.position));
                //Debug.Log(scal);
                correction[i] += new Vector3(scal, scal, scal);
            }
        correction[i] *= 1/rho0;
    }
}
