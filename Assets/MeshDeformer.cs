using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MeshDeformer : MonoBehaviour
{
    public GameObject targetCube;
    public GameObject followObject; // ��Ҫ���������
    public float intensity = 1f;
    public float angle = 45f;
    private float angle_pre;
    public Vector3 axis = Vector3.up;

    private Mesh mesh;
    private Vector3[] originalVertices;
    private Quaternion followObjectRotation; // ��¼��������ĳ�ʼ��ת

    public float T = 0f;
    public TextMeshProUGUI Sensor_Data; //������ʾsensor����
    int Sensor_Input;

    void Start()
    {
        mesh = targetCube.GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;

        angle_pre = angle;

        // ��¼��������ĳ�ʼ��ת
        followObjectRotation = followObject.transform.localRotation;
    }

    void FixedUpdate()
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, axis);

        Vector3[] deformedVertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];

            // Rotate the vertex if it belongs to the bottom face
            if (vertex.y < 0f)
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

        // ����ת����Ӧ���ڸ�������ľֲ���ת
        followObject.transform.localRotation = followObjectRotation * Quaternion.AngleAxis(angle, axis);


        // ��ȡsensor
        if (int.TryParse(Sensor_Data.text, out Sensor_Input))
        {
            //Debug.Log("Extracted number: " + number);
            // ��������Խ�number���������������������
            Sensor_Input = int.Parse(Sensor_Data.text);
            // the middle two varables are the limit of module input, final two varable are the parameter of curve
            T = MapIntToFloat(Sensor_Input, 600, 800, 0f, 15f);
            angle = angle_pre + T;
        }
        else
        {
            //Debug.Log("Failed to parse the number.");
        }
    }

    // Calculate the scaling factor for a vertex based on its original position and the current position
    float CalculateScalingFactor(Vector3 originalVertex)
    {
        Vector3 scaledOriginalVertex = Vector3.Scale(originalVertex, transform.localScale); // ���ǵ��������������
        Vector3 worldPosition = transform.TransformPoint(scaledOriginalVertex); // ���ֲ�����ת��Ϊ��������
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition); // ����������ת��Ϊ�ֲ�����

        float originalArea = Mathf.Abs(scaledOriginalVertex.x * scaledOriginalVertex.z);
        float currentArea = Mathf.Abs(localPosition.x * localPosition.z);
        return Mathf.Sqrt(originalArea / currentArea);
    }

    float MapIntToFloat(int intValue, int minValue, int maxValue, float minMappedValue, float maxMappedValue)
    {
        float mappedValue = Mathf.InverseLerp(minValue, maxValue, intValue);
        float result = Mathf.Lerp(minMappedValue, maxMappedValue, mappedValue);
        return result;
    }


}