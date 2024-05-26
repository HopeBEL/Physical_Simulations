using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateFluid : MonoBehaviour
{
    public GameObject fluidParticleObject;
    public int nbParticles = 20;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < nbParticles; i++) {
            Instantiate(fluidParticleObject, transform.position + new Vector3(Random.Range(-3f, 3f), 5f, Random.Range(-3f, 3f)), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
