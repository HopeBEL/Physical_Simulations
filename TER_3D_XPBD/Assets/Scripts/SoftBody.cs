using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBody : MonoBehaviour
{
    Mesh mesh;
    MeshCollider meshCollider;
    public Vector3[] vertices;
    // Edge : bon courage
    public int[] triangles;
    Vector3[] positions;
    public Vector3[] velocities;
    public float[] restVolumes;
    float[] edgeLengths;
    public float[] inverseMasses;
    Vector3[] gradients = new Vector3[12];
    public float edgeCompliance = 100f;
    public float volumeCompliance = 0f;
    List<Edge> edges = new List<Edge>();
    public int countEdge = 0;
    public int numberTet; 
    Vector3 gravity = new Vector3(0, -9.81f, 0);
    Vector3[] previousPos;
    float dt = 1/60f;
    Vector3[] volIdOrder = new Vector3[] {new Vector3(1, 3, 2),
                                            new Vector3(0, 2, 3),
                                            new Vector3(0, 3, 1),
                                            new Vector3(0, 1, 2)};

    //public Camera camera;
    RaycastHit hit;
    public bool isDraggingVertex = false;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
        vertices = mesh.vertices;
        triangles = mesh.triangles;
        mesh.vertices = vertices;
        numberTet = triangles.Length / (4 * 3);

        inverseMasses = new float[vertices.Length];
        restVolumes = new float[numberTet];
        velocities = new Vector3[vertices.Length];
        previousPos = new Vector3[vertices.Length];

        for (int i = 0; i < numberTet; i++) {
            float volume = GetTetVolume(i);
            restVolumes[i] = volume;
            float pInvMass = volume > 0f ? 1f/(volume/4f) : 0f;
            inverseMasses[triangles[4 * i]] += pInvMass; 
            inverseMasses[triangles[4 * i + 1]] += pInvMass; 
            inverseMasses[triangles[4 * i + 2]] += pInvMass; 
            inverseMasses[triangles[4 * i + 3]] += pInvMass; 

        }

        // On remplit edges de toutes les edges du Mesh
        GetMeshEdges();
        edgeLengths = new float[edges.Count];

        int j = 0;
        foreach (Edge e in edges) {
            Vector3 vertex1 = vertices[e.vertexId1];
            Vector3 vertex2 = vertices[e.vertexId2];

            // Dessiner une ligne entre les sommets de l'arête avec une couleur spécifique
            Debug.DrawLine(vertex1, vertex2, Color.red, 10.0f); // Changez Color.red selon votre besoin
       
            //edgeLengths[j] = Mathf.Sqrt(Mathf.Pow(vertex1[0] - vertex2[0], 2) + Mathf.Pow(vertex1[1] - vertex2[1], 2) + Mathf.Pow(vertex1[2] - vertex2[2], 2));
            edgeLengths[j] = Vector3.Distance(vertex1, vertex2);
            j++;
        }
    }

    void PreSolve() {
        for (int i = 0; i < vertices.Length; i++) {
            //Debug.Log("Presolve");
            if (inverseMasses[i] == 0f)
                continue;
            velocities[i] += gravity * dt;
            previousPos[i] = vertices[i];
            vertices[i] += velocities[i] * dt;

            if (vertices[i + 1][1] < 0f) {
                //Debug.Log("J'suis làààà");
                vertices[i] = previousPos[i];
                vertices[i + 1] = new Vector3(0f, 0f, 0f); //????
            }
        }
    }

    void Solve() {
        SolveEdges();
        SolveVolumes();
    }

    void PostSolve() {
        for (int i = 0; i < vertices.Length; i++) {
            if (inverseMasses[i] == 0f) continue;
            velocities[i] = vertices[i] - previousPos[i];
            velocities[i] /= dt;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        int nearestIndex = 0;

        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(ray, out hit)) {
                Debug.Log("J'ai tapé dans le Mesh");
                int vertexHitIndex1 = triangles[hit.triangleIndex * 3 + 0];
                int vertexHitIndex2 = triangles[hit.triangleIndex * 3 + 1];
                int vertexHitIndex3 = triangles[hit.triangleIndex * 3 + 2];

                int[] tab = new int[3] {vertexHitIndex1, vertexHitIndex2, vertexHitIndex3};
                // Trouver le vertex le + proche du Hit point
                float nearestDis = Vector3.Distance(vertices[vertexHitIndex1], hit.point);
                for (int i = 0; i < 3; i++) {
                    if (nearestDis > Vector3.Distance(vertices[tab[i]], hit.point)) {
                        nearestDis = Vector3.Distance(vertices[tab[i]], hit.point);
                        nearestIndex = tab[i];
                    }
                }
            }
            // Quand on clic gauche
            isDraggingVertex = true;
            vertices[nearestIndex] = Input.mousePosition;
        }*/

        PreSolve();
        Solve();
        PostSolve();

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.MarkModified();
    }

    void SolveEdges() {
        float alpha = edgeCompliance / dt / dt;

        for (int i = 0; i < countEdge; i++) {
            int v1 = edges[i].vertexId1;
            int v2 = edges[i].vertexId2;

            float w1 = inverseMasses[v1];
            float w2 = inverseMasses[v2];

            float w = w1 + w2;
            if (w == 0f) continue;

            gradients[0] = vertices[v1] - vertices[v2];
            float length = gradients[0].magnitude;
            if (length == 0f) continue;
            gradients[0] = gradients[0].normalized;

            float restLength = edgeLengths[i];
            float C = length - restLength;
            float lambda = -C / (w + alpha); // norme du gradient = 1?
            vertices[v1] +=  lambda*w1*gradients[0];
            vertices[v2] +=  -lambda*w2*gradients[0];
        }
    }

    void SolveVolumes() {
        float alpha = volumeCompliance / dt / dt;

        for (int i = 0; i < numberTet; i++) {
            float w = 0f;
            for (int j = 0; j < 4; j++) {
                int id1 = (int)triangles[4*i+(int)volIdOrder[j][0]];
                int id2 = (int)triangles[4*i+(int)volIdOrder[j][1]];
                int id3 = (int)triangles[4*i+(int)volIdOrder[j][2]];

                Vector3 temp1 = vertices[id2] - vertices[id1];
                Vector3 temp2 = vertices[id3] - vertices[id1];
                gradients[j] = Vector3.Cross(temp1, temp2);
                gradients[j] /= 6f;

                w += inverseMasses[triangles[4 * i + j]] * gradients[j].magnitude;
            }

            if (w == 0f) continue;
            float vol = GetTetVolume(i);
            float restVol = restVolumes[i];
            float C = vol - restVol;
            float lambda = -C / (w + alpha);

            for (int k = 0; k < 4; k++) {
                int id = triangles[4 * i + k];
                vertices[id] += lambda *inverseMasses[id] * gradients[k];
            }
        }
    }

    // Récupère le volume d'un tétrahèdre
    float GetTetVolume(int indTet) {
        int v1, v2, v3, v4;

        // On récupère les 4 vertices du tétrahèdre
        v1 = triangles[4 * indTet];
        v2 = triangles[4 * indTet + 1];
        v3 = triangles[4 * indTet + 2];
        v4 = triangles[4 * indTet + 3];

        Vector3 v21 = vertices[v2] - vertices[v1];
        Vector3 v31 = vertices[v3] - vertices[v1];
        Vector3 v41 = vertices[v4] - vertices[v1];

        return Vector3.Dot(Vector3.Cross(v21, v31), v41) / 6f;
    }

    public struct Edge {
        public int vertexId1;
        public int vertexId2;

        // Constructor
        public Edge(int v1, int v2) {
            vertexId1 = v1;
            vertexId2 = v2;
        }


    }

    void GetMeshEdges() {
        int vertexId1, vertexId2, vertexId3;
        // On parcourt tous les triangles
        for (int i = 0; i < triangles.Length; i += 3) {
            // Pour chaque triangle, on récupère les vertex le composant
            vertexId1 = triangles[i];
            vertexId2 = triangles[i+1];
            vertexId3 = triangles[i+2];

            // On ajoute les edges dans notre tableau
            Edge e1 = new Edge(vertexId1, vertexId2);
            Edge e2 = new Edge(vertexId2, vertexId3);
            Edge e3 = new Edge(vertexId3, vertexId1);
            edges.Add(e1);
            edges.Add(e2);
            edges.Add(e3);

            countEdge += 3;
            
        }
    }

    private void OnCollisionEnter(Collision other) {
        float minDis = Mathf.Infinity;
        int minIndex = 0;
        foreach(ContactPoint c in other.contacts) {
            for (int i = 0; i < vertices.Length; i++) {
                float d = Vector3.Distance(c.point, vertices[i]);
                if (d < minDis) {
                    minDis = d;
                    minIndex = i;
                }
            }
            
            vertices[minIndex] += c.point - vertices[minIndex] * 5f;
            mesh.vertices = vertices;
            meshCollider.sharedMesh = mesh;
        }
    }
}
