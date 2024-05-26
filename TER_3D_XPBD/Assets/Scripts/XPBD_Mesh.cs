using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A tester : mettre en place le grab de la souris pour déplacer les sommets
public class XPBD_Mesh : MonoBehaviour
{
    [Header("Mesh")]
    float[] verticesPos = new float[] {
        -0.277778f, -0.277778f, -0.277778f,
        0.277778f, -0.277778f, -0.277778f,
        0.277778f, -0.277778f, 0.277778f,
        -0.277778f, -0.277778f, 0.277778f,
        -0.277778f, 0.277778f, -0.277778f,
        0.277778f, 0.277778f, -0.277778f,
        0.277778f, 0.277778f, 0.277778f,
        -0.277778f, 0.277778f, 0.277778f,
        0f, -0.5f, 0f,
        0f, -0.375f, -0.375f,
        0.375f, -0.375f, 0f,
        0f, -0.375f, 0.375f,
        -0.375f, -0.375f, 0f,
        0f, 0f, 0.5f,
        0.375f, 0f, 0.375f,
        0f, 0.375f, 0.375f,
        -0.375f, 0f, 0.375f,
        0.5f, 0f, 0f,
        0.375f, 0f, -0.375f,
        0.375f, 0.375f, 0f,
        0f, 0f, -0.5f,
        -0.375f, 0f, -0.375f,
        0f, 0.375f, -0.375f,
        -0.5f, 0f, 0f,
        -0.375f, 0.375f, 0f,
        0f, 0.5f, 0f,
        /*-0.277778f, 0.7222219999999999f, -0.277778f,
        0.277778f, 0.7222219999999999f, -0.277778f,
        0.277778f, 0.7222219999999999f, 0.277778f,
        -0.277778f, 0.7222219999999999f, 0.277778f,
        -0.277778f, 1.277778f, -0.277778f,
        0.277778f, 1.277778f, -0.277778f,
        0.277778f, 1.277778f, 0.277778f,
        -0.277778f, 1.277778f, 0.277778f,
        0f, 0.5f, 0f,
        0f, 0.625f, -0.375f,
        0.375f, 0.625f, 0f,
        0f, 0.625f, 0.375f,
        -0.375f, 0.625f, 0f,
        0f, 1f, 0.5f,
        0.375f, 1f, 0.375f,
        0f, 1.375f, 0.375f,
        -0.375f, 1f, 0.375f,
        0.5f, 1f, 0f,
        0.375f, 1f, -0.375f,
        0.375f, 1.375f, 0f,
        0f, 1f, -0.5f,
        -0.375f, 1f, -0.375f,
        0f, 1.375f, -0.375f,
        -0.5f, 1f, 0f,
        -0.375f, 1.375f, 0f,
        0f, 1.5f, 0f*/
    }
    ;
    Vector3[] vertices;
    int[] tetrahedronIds = new int[] {
    5,1,20,18,
    19,6,5,17,
    0,4,21,23,
    4,0,7,23,
    24,4,7,23,
    12,0,23,3,
    0,7,23,3,
    23,7,16,3,
    3,11,2,13,
    0,7,3,2,
    9,0,1,20,
    7,3,2,13,
    7,16,3,13,
    14,2,13,6,
    0,4,7,6,
    0,7,2,6,
    7,15,13,6,
    2,7,13,6,
    4,24,7,25,
    7,15,6,25,
    4,7,6,25,
    0,4,6,5,
    25,4,22,5,
    19,6,25,5,
    6,4,25,5,
    1,5,17,18,
    1,2,10,17,
    0,9,1,8,
    2,0,6,1,
    6,0,5,1,
    12,0,3,8,
    2,1,10,8,
    2,0,1,8,
    3,0,2,8,
    11,3,2,8,
    5,6,1,17,
    14,2,6,17,
    6,2,1,17,
    4,0,21,20,
    0,5,1,20,
    4,22,5,20,
    0,4,5,20,
    /*31,27,46,44,
    45,32,31,43,
    26,30,47,49,
    30,26,33,49,
    50,30,33,49,
    38,26,49,29,
    26,33,49,29,
    49,33,42,29,
    29,37,28,39,
    26,33,29,28,
    35,26,27,46,
    33,29,28,39,
    33,42,29,39,
    40,28,39,32,
    26,30,33,32,
    26,33,28,32,
    33,41,39,32,
    28,33,39,32,
    30,50,33,51,
    33,41,32,51,
    30,33,32,51,
    26,30,32,31,
    51,30,48,31,
    45,32,51,31,
    32,30,51,31,
    27,31,43,44,
    27,28,36,43,
    26,35,27,34,
    28,26,32,27,
    32,26,31,27,
    38,26,29,34,
    28,27,36,34,
    28,26,27,34,
    29,26,28,34,
    37,29,28,34,
    31,32,27,43,
    40,28,32,43,
    32,28,27,43,
    30,26,47,46,
    26,31,27,46,
    30,48,31,46,
    26,30,31,46*/
    }
    ;
    Mesh ballMesh;
    MeshCollider meshCollider;
    public Material material;


    [Header("Attributes")]
    public Vector3[] velocities;
    public Vector3[] oldPositions;
    public float[] inverseMasses;
    public float[] restVolumes;
    public Vector3[] grads;
    List<Edge> edges = new List<Edge>();
    float[] edgeLengths;
    public int countEdge = 0;

    public float dt = 1/60f;
    public float k = 0.5f;
    public float alpha = 0f;
    public float restVolume = 0f;
    Vector3 gravity = new Vector3(0, -9.81f, 0);

    // Needed an Edge structure to store and go though all the edges of the mesh
    public struct Edge {
        public int vertexId1;
        public int vertexId2;

        // Constructor
        public Edge(int v1, int v2) {
            vertexId1 = v1;
            vertexId2 = v2;
        }
    }

    void Awake()
    {
        // Initialization of the vertices of the mesh
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        ballMesh = new Mesh();

        vertices = new Vector3[verticesPos.Length / 3];
        for (int i = 0; i < verticesPos.Length; i+= 3) {
            vertices[i / 3] = new Vector3(verticesPos[i], verticesPos[i + 1], verticesPos[i + 2]);
        }
        ballMesh.vertices = vertices;

        // Taking into account it's a tetrahedral mesh
        List<int> trianglesList = new List<int>();
        for (int i = 0; i < tetrahedronIds.Length; i += 4)
        {
            int v0 = tetrahedronIds[i];
            int v1 = tetrahedronIds[i + 1];
            int v2 = tetrahedronIds[i + 2];
            int v3 = tetrahedronIds[i + 3];

            // Face 1
            trianglesList.Add(v0);
            trianglesList.Add(v1);
            trianglesList.Add(v2);

            // Face 2
            trianglesList.Add(v0);
            trianglesList.Add(v2);
            trianglesList.Add(v3);

            // Face 3
            trianglesList.Add(v0);
            trianglesList.Add(v3);
            trianglesList.Add(v1);

            // Face 4
            trianglesList.Add(v1);
            trianglesList.Add(v3);
            trianglesList.Add(v2);
        }

        int[] triangles = trianglesList.ToArray();
        ballMesh.triangles = triangles;

        ballMesh.RecalculateNormals();
        ballMesh.RecalculateBounds();
        gameObject.GetComponent<MeshFilter>().mesh = ballMesh;
        gameObject.GetComponent<MeshRenderer>().material = material;

        velocities = new Vector3[vertices.Length];
        inverseMasses = new float[vertices.Length];
        oldPositions = new Vector3[vertices.Length];
        restVolumes = new float[tetrahedronIds.Length / 4];
        grads = new Vector3[tetrahedronIds.Length];
        alpha = 1/k;
        restVolume = GetMeshVolume();

        for (int i = 0; i < vertices.Length; i++)
        {
            inverseMasses[i] = 0.5f;
        }

        // Initialization of the edges
        GetMeshEdges(triangles);

        edgeLengths = new float[edges.Count];
        int j = 0;
        foreach (Edge e in edges) {
            Vector3 vertex1 = vertices[e.vertexId1];
            Vector3 vertex2 = vertices[e.vertexId2];

            // Draw a line just to check if the edges are okay
            Debug.DrawLine(vertex1, vertex2, Color.red, 10.0f);
       
            edgeLengths[j] = Vector3.Distance(vertex1, vertex2);
            j++;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int numSteps = 100;
        float sdt = dt / numSteps;
        for (int j = 0; j < numSteps; j++) {
            // Apply gravity forces and predict positions for each particles
            for (int i = 0; i < vertices.Length; i++) {
                velocities[i] += sdt * gravity;
                oldPositions[i] = vertices[i];
                vertices[i] += sdt * velocities[i];
            }

            // Solve constraints
            SolveDistanceConstraint(sdt);
            SolveVolumeConstraint(sdt);

            for (int i = 0; i < vertices.Length; i++) {
                SolveYConstraint(sdt, i);
            }

            // Update velocities
            for (int i = 0; i < vertices.Length; i++) {
                velocities[i] = (vertices[i] - oldPositions[i]) / sdt;
            }

            // Update mesh
            ballMesh.vertices = vertices;
            ballMesh.RecalculateNormals();
        }
    }

    // Go through all the triangles, get its vertices and adds the edges to the list
    void GetMeshEdges(int[] triangles) {
        int vertexId1, vertexId2, vertexId3;
        for (int i = 0; i < triangles.Length; i += 3) {
            vertexId1 = triangles[i];
            vertexId2 = triangles[i+1];
            vertexId3 = triangles[i+2];

            Edge e1 = new Edge(vertexId1, vertexId2);
            Edge e2 = new Edge(vertexId2, vertexId3);
            Edge e3 = new Edge(vertexId3, vertexId1);
            edges.Add(e1);
            edges.Add(e2);
            edges.Add(e3);

            countEdge += 3;
        }
    }

    // Go through all the tetrahedrons and get the volume of the mesh
    float GetMeshVolume() {
        float volume = 0f;
        for (int i = 0; i < tetrahedronIds.Length; i += 4) {
            Vector3 v1 = vertices[tetrahedronIds[i]];
            Vector3 v2 = vertices[tetrahedronIds[i+1]];
            Vector3 v3 = vertices[tetrahedronIds[i+2]];
            Vector3 v4 = vertices[tetrahedronIds[i+3]];
            float tetrahedronVolume = Mathf.Abs(Vector3.Dot(Vector3.Cross(v2 - v1, v3 - v1), v4 - v1) / 6f);
            volume += tetrahedronVolume;

            // Stores all the rest volumes too
            restVolumes[i / 4] = tetrahedronVolume;
        }
        return volume;
    }

    // Get the volume of one tetrahedron
    float GetTetVolume(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
        return Mathf.Abs(Vector3.Dot(Vector3.Cross(v2 - v1, v3 - v1), v4 - v1) / 6.0f);
    }

    // We suppose there's a spring between each pair of particles, thus a distance contraint between each pair
    void SolveDistanceConstraint(float dt) {
        float lambda = 0f;
        float C = 0f;

        // Iterate over all the edges
        for (int i = 0; i < edges.Count; i++) {
            Edge e = edges[i];
            float currentDistance = Vector3.Distance(vertices[e.vertexId1], vertices[e.vertexId2]);
            C = currentDistance - edgeLengths[i];
            lambda = - C /(inverseMasses[e.vertexId1] + inverseMasses[e.vertexId2] + (alpha / (dt * dt)));

            // Apply correction on both vertices of the current edge
            vertices[e.vertexId1] += lambda * inverseMasses[e.vertexId1] * -(vertices[e.vertexId2] - vertices[e.vertexId1]).normalized;
            vertices[e.vertexId2] += lambda * inverseMasses[e.vertexId2] * (vertices[e.vertexId2] - vertices[e.vertexId1]).normalized;
            }
        }

    // Volume constraint on the tetrahedrons
    void SolveVolumeConstraint(float dt) {
        float w = 0f;
        for (int i = 0; i < tetrahedronIds.Length / 4; i++) {
            // Get the vertices of the current tetrahedron
            Vector3 v1 = vertices[tetrahedronIds[4 * i]];
            Vector3 v2 = vertices[tetrahedronIds[4 * i + 1]];
            Vector3 v3 = vertices[tetrahedronIds[4 * i + 2]];
            Vector3 v4 = vertices[tetrahedronIds[4 * i + 3]];

            grads[0] = Vector3.Cross(v4 - v2, v3 - v2) * 1/6f;
            grads[1] = Vector3.Cross(v3 - v1, v4 - v1) * 1/6f;
            grads[2] = Vector3.Cross(v4 - v1, v2 - v1) * 1/6f;
            grads[3] = Vector3.Cross(v2 - v1, v3 - v1) * 1/6f;
          
            w = inverseMasses[tetrahedronIds[4 * i]] * grads[0].sqrMagnitude + inverseMasses[tetrahedronIds[4 * i + 1]] * grads[1].sqrMagnitude + inverseMasses[tetrahedronIds[4 * i + 2]] * grads[2].sqrMagnitude + inverseMasses[tetrahedronIds[4 * i + 3]] * grads[3].sqrMagnitude;

            // Get volume of current tetrahedron
            float volume = GetTetVolume(v1, v2, v3, v4);
            float lambdaVol = -(volume - restVolumes[i]) / (w + alpha / (dt * dt));

            // For each vertex of the current tetrahedron, update its position
            for (int j = 0; j < 4; j++) {
                vertices[tetrahedronIds[4 * i + j]] += lambdaVol * inverseMasses[tetrahedronIds[4 * i + j]] * grads[j].normalized;
                //Debug.Log("lambdaVol: " + lambdaVol + " inverseMasses: " + inverseMasses[tetrahedronIds[4 * i + j]] + " grads: " + grads[j] + " vertices: " + vertices[tetrahedronIds[4 * i + j]]);
            }
        }
    }

    // Bounce constraint
    public void SolveYConstraint(float sdt, int index) {
        float y = vertices[index].y;
        if (y <= 0) {
            Vector3 gradient = new Vector3(0, 1, 0);
            float lambda = -y / (inverseMasses[index] + (alpha / (sdt * sdt)));
            
            Vector3 deltaX = lambda * inverseMasses[index] * gradient;
            
            vertices[index] += deltaX;
        }
    }
}
