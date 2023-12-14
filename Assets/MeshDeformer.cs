
using UnityEngine;

public class MeshDeformer : MonoBehaviour
{
    public GameObject targetCube;
    public float intensity = 1f;
    public float angle = 45f;
    public Vector3 axis = Vector3.up;

    private Mesh mesh;
    private Vector3[] originalVertices;

    void Start()
    {
        mesh = targetCube.GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
    }

    void Update()
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, axis);

        Vector3[] deformedVertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];
            Vector3 vertexWorldPos = targetCube.transform.TransformPoint(vertex);

            // Rotate the vertex if it belongs to the bottom face
            if (vertex.y < 0f)
            {
                vertex = rotation * vertex;
            }

            // Calculate the scaling factor for each vertex based on its original and rotated positions
            float scale = CalculateScalingFactor(vertex, vertexWorldPos);

            // Scale the vertex position based on the scaling factor
            deformedVertices[i] = vertexWorldPos + (vertex - vertexWorldPos) * scale * intensity;
        }

        mesh.vertices = deformedVertices;
        mesh.RecalculateNormals();
    }

    // Calculate the scaling factor for a vertex based on its original position and the current position
    float CalculateScalingFactor(Vector3 originalVertex, Vector3 currentVertex)
    {
        float originalArea = Mathf.Abs(originalVertex.x * originalVertex.z);
        float currentArea = Mathf.Abs(currentVertex.x * currentVertex.z);
        return Mathf.Sqrt(originalArea / currentArea);
    }
}

/*
using UnityEngine;

public class MeshDeformer : MonoBehaviour
{
    public GameObject targetModel;
    public float intensity = 1f;
    public float angle = 45f;
    public Vector3 axis = Vector3.up;

    private Mesh mesh;
    private Vector3[] originalVertices;

    void Start()
    {
        mesh = targetModel.GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
    }

    void Update()
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, axis);

        Vector3[] deformedVertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];
            Vector3 vertexWorldPos = targetModel.transform.TransformPoint(vertex);

            // Rotate the vertex if it belongs to the bottom face
            if (vertex.y < 0f)
            {
                vertex = rotation * vertex;
            }

            // Calculate the scaling factor for each vertex based on its original and rotated positions
            float scale = CalculateScalingFactor(vertex, vertexWorldPos);

            // Scale the vertex position based on the scaling factor
            deformedVertices[i] = vertexWorldPos + (vertex - vertexWorldPos) * scale * intensity;
        }

        mesh.vertices = deformedVertices;
        mesh.RecalculateNormals();
    }

    // Calculate the scaling factor for a vertex based on its original position and the current position
    float CalculateScalingFactor(Vector3 originalVertex, Vector3 currentVertex)
    {
        float originalArea = Mathf.Abs(originalVertex.x * originalVertex.z);
        float currentArea = Mathf.Abs(currentVertex.x * currentVertex.z);
        return Mathf.Sqrt(originalArea / currentArea);
    }
}
*/