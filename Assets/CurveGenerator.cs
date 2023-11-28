using System.Collections.Generic;
using UnityEngine;

// 画出参数化曲线

public class CurveGenerator : MonoBehaviour
{
    public Transform[] controlPoints; // 输入点数组
    public int numPoints = 10; // 等距点数量

    private List<Vector3> curvePoints; // 曲线上的点列表
    private List<Vector3> normals; // 法向量列表

    private void Start()
    {
        curvePoints = new List<Vector3>();
        normals = new List<Vector3>();
    }

    private void FixedUpdate()
    {
        curvePoints.Clear();
        normals.Clear();

        // 构造曲线
        for (int i = 0; i < numPoints; i++)
        {
            float t = (float)i / (numPoints - 1);
            Vector3 point = GetPointOnCurve(t);
            curvePoints.Add(point);

            Vector3 normal = GetNormalOnCurve(t);
            normals.Add(normal);
        }

        // 在场景中绘制曲线
        for (int i = 0; i < curvePoints.Count - 1; i++)
        {
            Debug.DrawLine(curvePoints[i], curvePoints[i + 1], Color.red);
        }
    }



    // 获取曲线上指定参数值处的点
    private Vector3 GetPointOnCurve(float t)
    {
        Vector3 p0 = controlPoints[0].position;
        Vector3 p1 = controlPoints[1].position;
        Vector3 p2 = controlPoints[2].position;
        Vector3 p3 = controlPoints[3].position;

        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0;
        point += 3 * uu * t * p1;
        point += 3 * u * tt * p2;
        point += ttt * p3;

        return point;
    }

    // 获取曲线上指定参数值处的法向量
    private Vector3 GetNormalOnCurve(float t)
    {
        Vector3 tangent = GetTangentOnCurve(t);
        Vector3 binormal = Vector3.Cross(Vector3.up, tangent).normalized;
        Vector3 normal = Vector3.Cross(tangent, binormal).normalized;

        return normal;
    }

    // 获取曲线上指定参数值处的切向量
    private Vector3 GetTangentOnCurve(float t)
    {
        Vector3 p0 = controlPoints[0].position;
        Vector3 p1 = controlPoints[1].position;
        Vector3 p2 = controlPoints[2].position;
        Vector3 p3 = controlPoints[3].position;

        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 tangent = -3 * uu * p0;
        tangent += (3 * uu - 6 * u * t) * p1;
        tangent += (-3 * tt + 6 * t) * p2;
        tangent += 3 * tt * p3;

        return tangent.normalized;
    }
}