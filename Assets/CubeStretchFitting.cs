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
        // 定义文本内容
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
            // 使用正则表达式提取坐标信息
            string pattern = @"Cube group \d+; Cout \d+; cube " + i + @": \((-?\d+\.\d+), (-?\d+\.\d+), (-?\d+\.\d+)\)";
            System.Text.RegularExpressions.MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(text, pattern);

            // 将匹配到的坐标添加到spacePoints列表中
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                float x = float.Parse(match.Groups[1].Value);
                float y = float.Parse(match.Groups[2].Value);
                float z = float.Parse(match.Groups[3].Value);
                spacePoints.Add(new Vector3(x, y, z));
            }

            ///把读取的位置拟合   
            trajectoryPoints[i] = FitTrajectory(spacePoints);

            // 打印结果
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
        List<Vector3> output_cube_position = new List<Vector3>(); // 清理原来的output_cube_position
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
    ///这个函数接受拟合后的轨迹点列表trajectoryPoints和T值作为参数。根据T值，计算出对应的轨迹点索引index。
    ///然后返回该索引对应的轨迹点位置。在这个函数中，我们使用Mathf.RoundToInt函数将浮点数T乘以轨迹点数量减1并四舍五入，得到最接近T对应的索引值。然后通过索引来获取相应的轨迹点位置。
    public Vector3 GetTrajectoryPoint(List<Vector3> trajectoryPoints, float T)
    {
        int index = Mathf.RoundToInt(T * (trajectoryPoints.Count - 1));
        return trajectoryPoints[index];
    }



    ////////////////////////////////////////////////////////////////////////////////////////
    ///FitTrajectory函数用于拟合经过给定空间点的贝塞尔曲线轨迹，返回一个包含轨迹上的点的列表。

    ///这段完整的代码定义了一个BezierCurveFitting类，其中包含了FitTrajectory方法用于拟合轨迹。在FitTrajectory方法中，我们首先根据输入的已知轨迹点列表spacePoints，计算出控制点列表controlPoints。
    ///然后根据控制点列表，使用Bezier曲线的计算方法CalculateBezierPoint计算出拟合的轨迹点，并将其添加到trajectoryPoints列表中。最后返回拟合后的轨迹点列表trajectoryPoints。

    ///CalculateBezierPoint方法根据控制点列表和参数t计算Bezier曲线上的点。BinomialCoefficient方法用于计算二项式系数。
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
        float deltaT = 1.0f / 99; // 计算T的等距间隔，使得拟合的轨迹点有100个
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
