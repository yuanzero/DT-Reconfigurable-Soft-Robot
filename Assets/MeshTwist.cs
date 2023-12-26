using UnityEngine;

public class MeshTwist : MonoBehaviour
{
    public float angle = 45f;
    public float intensity = 1f;
    public Vector3 axis = Vector3.up;

    private Mesh mesh;
    private Vector3[] originalVertices;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
    }

    void FixedUpdate()
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, axis);

        Vector3[] deformedVertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];

            // Rotate the vertex if it belongs to the top face
            if (vertex.y > 0f)
            {
                vertex = rotation * vertex;
            }

            // Calculate the scaling factor for each vertex based on its original and rotated positions
            float scale = CalculateScalingFactor(vertex);

            // Scale the vertex position based on the scaling factor
            deformedVertices[i] = vertex * scale * intensity;
        }

        mesh.vertices = deformedVertices;
        mesh.RecalculateNormals();
    }

    float CalculateScalingFactor(Vector3 originalVertex)
    {
        // Add your custom logic for calculating the scaling factor here
        Vector3 currentVertex = transform.TransformPoint(originalVertex);
        float originalArea = Mathf.Abs(originalVertex.x * originalVertex.z);
        float currentArea = Mathf.Abs(currentVertex.x * currentVertex.z);
        return Mathf.Sqrt(originalArea / currentArea);
    }
}
