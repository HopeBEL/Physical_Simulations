using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBounce : MonoBehaviour
{

    public GameObject particle;
    Vector3 gravity = new Vector3(0, -9.81f, 0);
    public float dt = 1/60f;
    public Vector3 position;
    public float mass = 5f;
    public float inverseMass = 0f;
    public Vector3 velocitiy;
    public float k = 1f;
    public float alpha;
    public Vector3 gradient;

    // Start is called before the first frame update
    void Start()
    {
        inverseMass = 1 / mass;   
        alpha = 1 / k;     
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int numSteps = 100;
        float sdt = dt / numSteps;
        for (int j = 0; j < numSteps; j++) {
            // Apply gravity forces and predict position
            velocitiy += gravity * sdt;
            position = transform.position;
            transform.position += sdt * velocitiy;

            // Solve the constraint : here that y > 0
            SolveYConstraint(sdt);

            // Update velocity
            velocitiy = (transform.position - position) / sdt;
        }
    }

    public void SolveYConstraint(float sdt) {
        float y = transform.position.y;
        if (y <= 0) {
            gradient = new Vector3(0, 1, 0);
            float lambda = -y / (inverseMass + (alpha / (sdt * sdt))); // XPBD : stable even with high values of k
            //float lambda = -y / (inverseMass); PBD : unstable with high values of k
            
            // Calculate position correction
            Vector3 deltaX = lambda * inverseMass * gradient; // XPBD
            // Vector3 deltaX = k * lambda * inverseMass * gradient; PBD
            
            // Apply position correction
            transform.position += k*deltaX;
        }
    }
}
