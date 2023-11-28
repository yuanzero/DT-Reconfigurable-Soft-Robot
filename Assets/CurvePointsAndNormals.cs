using System.Collections.Generic;
using UnityEngine;

public class CurvePointsAndNormals : MonoBehaviour
{
    public int numPoints = 10; // 等距点的数量
    public GameObject pointPrefab; // 用于表示等距点的预制体
    public GameObject normalPrefab; // 用于表示法向量的预制体
    public float normalLength = 0.5f; // 法向量的长度
    public GameObject line; // 曲线

    private LineRenderer lineRenderer; // 用于绘制曲线的LineRenderer组件
    private List<GameObject> points; // 存储等距点的列表
    private List<GameObject> normals; // 存储法向量的列表

    void Start()
    {
        lineRenderer = line.GetComponent<LineRenderer>();
        points = new List<GameObject>();
        normals = new List<GameObject>();

        // 计算曲线长度
        float length = 0f;
        Vector3 prevPoint = lineRenderer.GetPosition(0);
        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            Vector3 currPoint = lineRenderer.GetPosition(i);
            length += Vector3.Distance(prevPoint, currPoint);
            prevPoint = currPoint;
        }

        // 计算等距点的间隔距离
        float interval = length / (numPoints - 1);

        // 计算等距点的坐标和法向量
        for (int i = 0; i < numPoints; i++)
        {
            // 计算当前点的位置和切线向量
            float distance = i * interval;
            Vector3 point = GetPointAtDistance(distance);
            Vector3 tangent = GetTangentAtDistance(distance);

            // 计算法向量
            Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;

            // 创建等距点和法向量的GameObject
            GameObject pointObj = Instantiate(pointPrefab, point, Quaternion.identity);
            GameObject normalObj = Instantiate(normalPrefab, point + normal * normalLength, Quaternion.identity);

            // 将等距点和法向量加入列表
            points.Add(pointObj);
            normals.Add(normalObj);
        }
    }

    void Update()
    {
        // 更新等距点和法向量的位置和方向
        for (int i = 0; i < numPoints; i++)
        {
            float distance = i * (lineRenderer.positionCount - 1) / (numPoints - 1);
            Vector3 point = GetPointAtDistance(distance);
            Vector3 tangent = GetTangentAtDistance(distance);
            Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;

            points[i].transform.position = point;
            normals[i].transform.position = point + normal * normalLength;
            normals[i].transform.rotation = Quaternion.LookRotation(normal, tangent);
        }
    }

    // 根据距离获取曲线上的点坐标
    Vector3 GetPointAtDistance(float distance)
    {
        float length = 0f;
        Vector3 prevPoint = lineRenderer.GetPosition(0);
        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            Vector3 currPoint = lineRenderer.GetPosition(i);
            float segmentLength = Vector3.Distance(prevPoint, currPoint);
            if (length + segmentLength >= distance)
            {
                float t = (distance - length) / segmentLength;
                return Vector3.Lerp(prevPoint, currPoint, t);
            }
            length += segmentLength;
            prevPoint = currPoint;
        }
        return prevPoint;
    }

    // 根据距离获取曲线上的切线向量
    Vector3 GetTangentAtDistance(float distance)
    {
        float length = 0f;
        Vector3 prevPoint = lineRenderer.GetPosition(0);
        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            Vector3 currPoint = lineRenderer.GetPosition(i);
            float segmentLength = Vector3.Distance(prevPoint, currPoint);
            if (length + segmentLength >= distance)
            {
                float t = (distance - length) / segmentLength;
                return Vector3.Lerp(prevPoint, currPoint, t) - prevPoint;
            }
            length += segmentLength;
            prevPoint = currPoint;
        }
        return prevPoint - lineRenderer.GetPosition(lineRenderer.positionCount - 2);
    }
}