using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


/* This class is used to manipulate the interpolation/lerp speed between current state and the 
 * latest via Mqtt - DTStacks received pose message. 
 * This lerping procedure is an effective way to limit network traffic and overloading of the Unity-environment 
 * if multiple (different) soft-robots shall be visualized.
 * 
 * This is intended to find suitable balancing values while the application remains within the unity editor, 
 * hence, no UWP version is made. The determined values need to be entered as fixed to the joints before building the final application.
 */


public class PositionSliderHandler_new : MonoBehaviour
{
    //Get reference to the UI elements
    public Slider positionSlider;
    public Text positionValueText;

    public Slider RotationSlider;
    public Text RotationValueText;

    // Set true if using slider control
    [Tooltip("Set true if using slider control")]
    public bool isSliderControl = true;

    // Set true if using slider control
    [Tooltip("Set true if using curve control")]
    public bool isCurveControl = false;

    // set reference coor for cureve
    public Transform Curve;


    // Get reference to all individual joint handlers
    public SoftgripperJointHandler[] jointHandler;


    // 曲线参数
    public Transform[] controlPoints; // 输入点数组
    public int numPoints = 10; // 等距点数量

    private List<Vector3> curvePoints; // 曲线上的点列表
    private List<Vector3> normals; // 法向量列表

    private void Start()
    {
        curvePoints = new List<Vector3>();
        normals = new List<Vector3>();
    }

    void FixedUpdate()
    {
        
        //
        curvePoints.Clear();
        normals.Clear();

        // 构造曲线方法2

        Vector3 p0 = controlPoints[0].localPosition;
        Vector3 p1 = controlPoints[1].localPosition;
        Vector3 p2 = controlPoints[2].localPosition;
        Vector3 p3 = controlPoints[3].localPosition;

        BezierCurve curve = new BezierCurve(p0, p1, p2, p3);

        

        // 构造曲线
        for (int i = 0; i < numPoints; i++)
        {
            float t = (float)i / (numPoints - 1);
            Vector3 point = GetPointOnCurve(t);
            Vector3 targetPos = Curve.TransformPoint(point);
            curvePoints.Add(targetPos);

            //Vector3 normal_angle = GetNormalOnCurve(t);  //方法1
            Vector3 normal_angle = curve.GetAngleWithAllAxis(t); //方法2

            normal_angle = Angle_process(normal_angle);

            // 连续性判断
            if (normals.Count > 0) 
            { 
                float difference = normals[normals.Count - 1].y - normal_angle.y; 
                if (difference < -90f || difference > 90f)
                { 
                    normal_angle.y = normal_angle.y + 180f; 
                }
               
                float difference_z = normals[normals.Count - 1].z - normal_angle.z;
                if (difference_z > 90f || difference_z < -90f)
                {
                    normal_angle.z = normal_angle.z + 180f;
                }
            }

            // Debug.Log("normal_new" + normal_angle);

            normals.Add(normal_angle);

        }



        

        // 在场景中绘制曲线
        for (int i = 0; i < curvePoints.Count - 1; i++)
        {
            Debug.DrawLine(curvePoints[i], curvePoints[i + 1], Color.red);
        }

        // slider controller
        int ii = 0;
        //update each joint handler lerp speed with the slider values
        foreach (SoftgripperJointHandler handler in jointHandler)
        {
            //handler.isSliderControl = isSliderControl;

            Vector3 Eularangle = handler.transform.eulerAngles;

            if (isSliderControl)
            {
                //handler.positionMsg = new Vector3((float)Mathf.Round((positionSlider.value - 0.5f) * 1000f) / 1000f + handler.xOffset, 0f + handler.yOffset, 0f);
                handler.positionMsg = new Vector3((float)handler.initPosition.x, (1f + (positionSlider.value - 0.5f)) * handler.initPosition.y, handler.initPosition.z);
                handler.rotationMsg = new Vector3((float)RotationSlider.value, 0, 0);
            }
            else if (isCurveControl)
            {
                handler.positionMsg = curvePoints[ii];
                handler.rotationMsg = normals[ii];
            }
            ii++;
        }
        //update the textbox next to the sliders with the slider value
        positionValueText.text = positionSlider.value.ToString();
        RotationValueText.text = RotationSlider.value.ToString();
    }


    // 获取曲线上指定参数值处的点
    private Vector3 GetPointOnCurve(float t)
    {
        /*
        Vector3 p0 = controlPoints[0].position;
        Vector3 p1 = controlPoints[1].position;
        Vector3 p2 = controlPoints[2].position;
        Vector3 p3 = controlPoints[3].position;
        */

        
        Vector3 p0 = controlPoints[0].localPosition;
        Vector3 p1 = controlPoints[1].localPosition;
        Vector3 p2 = controlPoints[2].localPosition;
        Vector3 p3 = controlPoints[3].localPosition;
        


        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0;
        point += 3 * uu * t * p1;
        point += 3 * u * tt * p2;
        point += ttt * p3;

        //point = controlPoints[0].transform.InverseTransformPoint(curvePoints[ii]);

        return point;
    }

    // 获取曲线上指定参数值处的法向量
    private Vector3 GetNormalOnCurve(float t)
    {
        Vector3 tangent = GetTangentOnCurve(t);


        //Vector3 binormal = new Vector3(1, 0, 0);
        Vector3 binormal = Curve.right;

        //Vector3 binormal = Vector3.Cross(tangent, Vector3.up).normalized;
        Vector3 normal = Vector3.Cross(binormal,tangent).normalized;

        // 判断法向量方向是否与切线夹角为逆时针旋转90度
        Vector2 tangent2D = new Vector2(tangent.y, tangent.z).normalized;
        Vector2 normal2D = new Vector2(normal.y, normal.z).normalized;
        float cross = tangent2D.x * normal2D.y - tangent2D.y * normal2D.x;
        if (cross < 0)
        {
            normal = -normal; // 反转法向量方向
        }



        /*
         //float xAngle = Vector3.SignedAngle(normal, Vector3.right, Vector3.up);
         float yAngle = Vector3.SignedAngle(normal, Vector3.up, Vector3.right);
         float zAngle = Vector3.SignedAngle(normal, Vector3.forward, Vector3.up);


         //xAngle = 0f;
         yAngle = 0f;
         zAngle = 0f;
        */

        // 计算法向量在y-z平面上的投影向量
        Vector3 projection = new Vector3(0, normal.y, normal.z);
        projection.Normalize(); // 将投影向量归一化处理

        // 计算物体绕x轴旋转的角度
        float xAngle = Vector3.SignedAngle(Vector3.up, projection, Vector3.right)+90f;

        // 计算法向量在x-z平面上的投影向量
        Vector3 projection_xz = new Vector3(normal.x, 0, normal.z);
        projection_xz.Normalize(); // 将投影向量归一化处理

        // 计算物体绕y轴旋转的角度
        float yAngle = Vector3.SignedAngle(Vector3.forward, projection_xz, Vector3.up);
        //float yAngle = 0f;

        // 计算法向量在x-y平面上的投影向量
        Vector3 projection_xy = new Vector3(normal.x, normal.y, 0);
        projection_xy.Normalize(); // 将投影向量归一化处理

        // 计算物体绕z轴旋转的角度
        //float zAngle = Vector3.SignedAngle(Vector3.up, projection_xy, Vector3.forward);
        float zAngle = 0f;

        /*
        // 处理角度值，确保其在[-180, 180]之间
        if (yAngle > 180f)
        {
            yAngle -= 360f;
        }
        else if (yAngle < -180f)
        {
            yAngle += 360f;
        }

        //保证旋转对称
        if (yAngle > 90f)
        {
            yAngle -= 180f;
        }
        else if (yAngle < -90f)
        {
            yAngle += 180f;
        }


        if (xAngle > 180f)
        {
            xAngle -= 360f;
        }
        else if (xAngle < -180f)
        {
            xAngle += 360f;
        }

        if (zAngle > 180f)
        {
            zAngle -= 360f;
        }
        else if (zAngle < -180f)
        {
            zAngle += 360f;
        }
        */

        Vector3 angles = new Vector3(xAngle, yAngle, zAngle);
        Debug.Log("Angles: " + angles);

        return angles;
    }

    // 获取曲线上指定参数值处的切向量
    private Vector3 GetTangentOnCurve(float t)
    {
        /*
        Vector3 p0 = controlPoints[0].position;
        Vector3 p1 = controlPoints[1].position;
        Vector3 p2 = controlPoints[2].position;
        Vector3 p3 = controlPoints[3].position;
        */


        Vector3 p0 = controlPoints[0].localPosition;
        Vector3 p1 = controlPoints[1].localPosition;
        Vector3 p2 = controlPoints[2].localPosition;
        Vector3 p3 = controlPoints[3].localPosition;

        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 tangent = -3 * uu * p0;
        tangent += (3 * uu - 6 * u * t) * p1;
        tangent += (-3 * tt + 6 * u * t) * p2;
        tangent += 3 * tt * p3;

        return tangent.normalized;
    }


    // 方法2构造曲线
    public class BezierCurve
    {
        private Vector3[] controlPoints;

        public BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            controlPoints = new Vector3[] { p0, p1, p2, p3 };
        }

        public Vector3 GetTangent(float t)
        {
            Vector3 tangent = 3 * (1 - t) * (1 - t) * (controlPoints[1] - controlPoints[0])
                            + 6 * (1 - t) * t * (controlPoints[2] - controlPoints[1])
                            + 3 * t * t * (controlPoints[3] - controlPoints[2]);
            return Vector3.Normalize(tangent);
        }

        public Vector3 GetNormal(float t)
        {
            Vector3 tangent = GetTangent(t);
            /*
             * 求解法向量的第一种方法，因为法向量有无数个，法平面唯一，需要验证所求
            Vector3 normal_sub = 6 * (1 - t) * (controlPoints[2] - 2 * controlPoints[1] + controlPoints[0])
                           + 6 * t * (controlPoints[3] - 2 * controlPoints[2] + controlPoints[1]);
            Vector3 normal_main = Vector3.Normalize(Vector3.Cross(tangent, normal_sub)); //outside the plane
            Vector3 normal = Vector3.Normalize(Vector3.Cross(tangent, normal_main));


            // 判断法向量方向是否与切线夹角为逆时针旋转90度
            Vector2 tangent2D = new Vector2(tangent.y, tangent.z).normalized;
            Vector2 normal2D = new Vector2(normal.y, normal.z).normalized;
            float cross = tangent2D.x * normal2D.y - tangent2D.y * normal2D.x;
            if (cross < 0)
            {
                normal = -normal; // 反转法向量方向
            }
            */

            // 求解法向量的第二种方法,直接逆时针绕x轴旋转90度
            Vector3 normal = Quaternion.Euler(-90, 0, 0) * tangent;

            return normal;
        }

        public float GetAngleWithXAxis(float t)
        {
            Vector3 normal = GetNormal(t);
            // 计算法向量在y-z平面上的投影向量
            Vector3 projection_yz = new Vector3(0, normal.y, normal.z);
            projection_yz.Normalize(); // 将投影向量归一化处理
            return Vector3.SignedAngle(projection_yz, new Vector3(0, 1, 0), new Vector3(-1, 0, 0))-90f;
        }

        public float GetAngleWithYAxis(float t)
        {
            Vector3 normal = GetNormal(t);
            // 计算法向量在x-z平面上的投影向量
            Vector3 projection_xz = new Vector3(normal.x, 0, normal.z);
            projection_xz.Normalize(); // 将投影向量归一化处理

            return Vector3.SignedAngle(projection_xz, new Vector3(1, 0, 0), new Vector3(0, 1, 0)) + 90f;
        }

        public float GetAngleWithZAxis(float t)
        {
            Vector3 normal = GetNormal(t);
            // 计算法向量在x-y平面上的投影向量
            Vector3 projection_xy = new Vector3(normal.x, normal.y, 0);
            projection_xy.Normalize(); // 将投影向量归一化处理
            //float angle = Vector3.SignedAngle(projection_xy, new Vector3(0, 1, 0), new Vector3(0, 0, 1));
            return Vector3.SignedAngle(projection_xy, new Vector3(0, 1, 0), new Vector3(0, 0, 1))-180f;
            //return (float)Math.Acos(Vector3.Dot(normal, new Vector3(0, 0, 1)));
        }

        public Vector3 GetAngleWithAllAxis(float t)
        {
            //Vector3 normal_angle = new Vector3(GetAngleWithXAxis(t), 0, 0); // very stable
            // Vector3 normal_angle = new Vector3(GetAngleWithXAxis(t), 0, GetAngleWithZAxis(t)); // stable
             Vector3 normal_angle = new Vector3(GetAngleWithXAxis(t), GetAngleWithYAxis(t), GetAngleWithZAxis(t)); // not stable
            //normal_angle = Vector3.Scale(normal_angle, new Vector3((float)(180f /Math.PI), (float)(180f / Math.PI), (float)(180f / Math.PI)));
            return normal_angle;
        }
    }

    public Vector3 Angle_process(Vector3 angle) 
    { 
        if (angle.x > 180f) 
        { angle.x -= 360f; } 
        else if (angle.x < -180f) 
        { angle.x += 360f; } 
        if (angle.y > 180f) 
        { angle.y -= 360f; } 
        else if (angle.y < -180f) 
        { angle.y += 360f; } 
        if (angle.z > 180f) { angle.z -= 360f; } 
        else if (angle.z < -180f) 
        { angle.z += 360f; } 
        return angle; 
    }
        
}
