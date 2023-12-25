using UnityEngine;
using System.Collections.Generic;

public class CubeStretchFitting : MonoBehaviour
{
    public int cubeJ = 4; //the number of control points
    //public int CoutI;
    public List<Vector3> spacePoints;
    List<Vector3>[] trajectoryPoints;
    //List<Vector3> output_cube_position;

    void Start()
    {
        trajectoryPoints = new List<Vector3>[cubeJ];
        for (int i = 0; i < cubeJ; i++)
        {
            trajectoryPoints[i] = new List<Vector3>();
        }
        // �����ı�����
        // bending
        string text = @"
        Cube group 0; Cout 1; cube 0: (0.0, -2.19, 0.0)
        Cube group 0; Cout 1; cube 1: (0.0, -2.0, -1.4)
        Cube group 0; Cout 1; cube 2: (0.0, -2.0, -2.8)
        Cube group 0; Cout 1; cube 3: (0.0, -2.09, -4.8)
        Cube group 0; Cout 2; cube 0: (0.0, -2.1, 0.0)
        Cube group 0; Cout 2; cube 1: (0.0, -2.0, -2.6)
        Cube group 0; Cout 2; cube 2: (0.0, -2.0, -4.1)
        Cube group 0; Cout 2; cube 3: (0.0, -2.09, -6.0)
        Cube group 0; Cout 3; cube 0: (0.0, -2.19, 0.0)
        Cube group 0; Cout 3; cube 1: (0.0, -2.0, -3.4)
        Cube group 0; Cout 3; cube 2: (0.0, -2.0, -5.9)
        Cube group 0; Cout 3; cube 3: (0.0, -2.09, -8.0)
        Cube group 0; Cout 4; cube 0: (0.0, -2.19, 0.0)
        Cube group 0; Cout 4; cube 1: (0.0, -2.0, -3.5)
        Cube group 0; Cout 4; cube 2: (0.0, -2.0, -6.8)
        Cube group 0; Cout 4; cube 3: (0.0, -2.09, -10.0)
        Cube group 0; Cout 5; cube 0: (0.0, -2.19, 0.0)
        Cube group 0; Cout 5; cube 1: (0.0, -2.0, -4.3)
        Cube group 0; Cout 5; cube 2: (0.0, -2.0, -8.4)
        Cube group 0; Cout 5; cube 3: (0.0, -2.09, -12.0)
        ";

        for (int i = 0; i < cubeJ; i++)
        {
            spacePoints.Clear();
            // ʹ��������ʽ��ȡ������Ϣ
            string pattern = @"Cube group \d+; Cout \d+; cube " + i + @": \((-?\d+\.\d+), (-?\d+\.\d+), (-?\d+\.\d+)\)";
            System.Text.RegularExpressions.MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(text, pattern);

            // ��ƥ�䵽��������ӵ�spacePoints�б���
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                float x = float.Parse(match.Groups[1].Value);
                float y = float.Parse(match.Groups[2].Value);
                float z = float.Parse(match.Groups[3].Value);
                spacePoints.Add(new Vector3(x, y, z));
            }

            ///�Ѷ�ȡ��λ�����   
            trajectoryPoints[i] = FitTrajectory(spacePoints);

            // ��ӡ���
            foreach (Vector3 point in trajectoryPoints[i])
            {
                Debug.Log(point);
            }
        }
    }

    /// <summary>
    /// get the position of the fitting Trajectory for a cube
    /// </summary>
    /// <param name="cubeJ"></param> the idex of cube
    /// <param name="T"></param> the parameter for bezier curve
    /// <returns></returns>
    public List<Vector3> GetTrajectoryPoint_CUBE(float T)
    {
        List<Vector3> output_cube_position = new List<Vector3>(); // ����ԭ����output_cube_position
        if (trajectoryPoints != null)
        {
            for (int i = 0; i < cubeJ; i++)
            {

                Vector3 tem = GetTrajectoryPoint(trajectoryPoints[i], T);
                output_cube_position.Add(GetTrajectoryPoint(trajectoryPoints[i], T));
                //Debug.Log("output = " + tem);
            }
        }
        return output_cube_position;
    }


    ////////////////////////////////////////////////////////////////////////
    ///�������������Ϻ�Ĺ켣���б�trajectoryPoints��Tֵ��Ϊ����������Tֵ���������Ӧ�Ĺ켣������index��
    ///Ȼ�󷵻ظ�������Ӧ�Ĺ켣��λ�á�����������У�����ʹ��Mathf.RoundToInt������������T���Թ켣��������1���������룬�õ���ӽ�T��Ӧ������ֵ��Ȼ��ͨ����������ȡ��Ӧ�Ĺ켣��λ�á�
    public Vector3 GetTrajectoryPoint(List<Vector3> trajectoryPoints, float T)
    {
        int index = Mathf.RoundToInt(T * (trajectoryPoints.Count - 1));
        return trajectoryPoints[index];
    }



    ////////////////////////////////////////////////////////////////////////////////////////
    ///FitTrajectory����������Ͼ��������ռ��ı��������߹켣������һ�������켣�ϵĵ���б�

    ///��������Ĵ��붨����һ��BezierCurveFitting�࣬���а�����FitTrajectory����������Ϲ켣����FitTrajectory�����У��������ȸ����������֪�켣���б�spacePoints����������Ƶ��б�controlPoints��
    ///Ȼ����ݿ��Ƶ��б�ʹ��Bezier���ߵļ��㷽��CalculateBezierPoint�������ϵĹ켣�㣬��������ӵ�trajectoryPoints�б��С���󷵻���Ϻ�Ĺ켣���б�trajectoryPoints��

    ///CalculateBezierPoint�������ݿ��Ƶ��б�Ͳ���t����Bezier�����ϵĵ㡣BinomialCoefficient�������ڼ������ʽϵ����
    ///
    public List<Vector3> FitTrajectory(List<Vector3> spacePoints)
    {
        int n = spacePoints.Count - 1;
        List<Vector3> controlPoints = new List<Vector3>();
        for (int i = 0; i <= n; i++)
        {
            controlPoints.Add(spacePoints[i]);
        }

        List<Vector3> trajectoryPoints = new List<Vector3>();
        float deltaT = 1.0f / 99; // ����T�ĵȾ�����ʹ����ϵĹ켣����100��
        for (float t = 0; t <= 1; t += deltaT)
        {
            Vector3 point = CalculateBezierPoint(controlPoints, t);
            trajectoryPoints.Add(point);
        }

        return trajectoryPoints;
    }

    private Vector3 CalculateBezierPoint(List<Vector3> controlPoints, float t)
    {
        int n = controlPoints.Count - 1;
        Vector3 point = Vector3.zero;
        for (int i = 0; i <= n; i++)
        {
            float coefficient = BinomialCoefficient(n, i) * Mathf.Pow(t, i) * Mathf.Pow(1 - t, n - i);
            point += coefficient * controlPoints[i];
        }

        return point;
    }

    private int BinomialCoefficient(int n, int k)
    {
        int result = 1;
        for (int i = 1; i <= k; i++)
        {
            result *= n - i + 1;
            result /= i;
        }
        return result;
    }
}
