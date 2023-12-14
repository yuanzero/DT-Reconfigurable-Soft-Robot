using UnityEngine;
using System.Collections.Generic;

public class CubeMover : MonoBehaviour
{
    // 声明一个变量来存储另一个脚本的实例
    public CubeSpacePointsFitting cubeSpacePointsFitting;
    // 声明一个游戏对象数组来存储需要更新位置的cube
    public GameObject[] cubes;
    public float T = 0.5f;
    List<Vector3> trajectoryPoints;

    void Start()
    {
        
    }
    void FixedUpdate()
    {
        T = Mathf.Clamp(T, 0f, 1f);
        // 调用另一个脚本的方法获取轨迹点列表
        trajectoryPoints = cubeSpacePointsFitting.GetTrajectoryPoint_CUBE(T);

        // 更新每个cube的位置
        for (int i = 0; i < cubes.Length; i++)
        {
            // 检查轨迹点列表的索引是否超出范围
            if (i < trajectoryPoints.Count)
            {
                // 更新cube的局部位置为对应的轨迹点
                cubes[i].transform.localPosition = trajectoryPoints[i];
            }
            else
            {
                // 如果轨迹点列表的索引超出范围，则将cube的局部位置不变
            }
        }
    }
}