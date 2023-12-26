using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MeshDeformer : MonoBehaviour
{
    public GameObject targetCube;
    public GameObject followObject; // 需要跟随的物体
    public float intensity = 1f;
    public float angle = 45f;
    private float angle_pre;
    public Vector3 axis = Vector3.up;

    private Mesh mesh;
    private Vector3[] originalVertices;
    private Quaternion followObjectRotation; // 记录跟随物体的初始旋转

    public float T = 0f;
    public TextMeshProUGUI Sensor_Data; //用于显示sensor输入
    int Sensor_Input;

    void Start()
    {
        mesh = targetCube.GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;

        angle_pre = angle;

        // 记录跟随物体的初始旋转
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

        // 将旋转操作应用于跟随物体的局部旋转
        followObject.transform.localRotation = followObjectRotation * Quaternion.AngleAxis(angle, axis);


        // 读取sensor
        if (int.TryParse(Sensor_Data.text, out Sensor_Input))
        {
            //Debug.Log("Extracted number: " + number);
            // 在这里可以将number保存下来或进行其他处理
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
        Vector3 scaledOriginalVertex = Vector3.Scale(originalVertex, transform.localScale); // 考虑到物体的缩放因子
        Vector3 worldPosition = transform.TransformPoint(scaledOriginalVertex); // 将局部坐标转换为世界坐标
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition); // 将世界坐标转换为局部坐标

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