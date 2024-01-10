using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;

public class CubeMover : MonoBehaviour
{
    // 声明一个变量来存储另一个脚本的实例
    public CubeSpacePointsFitting cubeSpacePointsFitting;
    // 声明一个游戏对象数组来存储需要更新位置的cube
    public GameObject[] cubes;
    public float T = 0.5f;
    List<Vector3> trajectoryPoints;
    public TextMeshProUGUI Sensor_Data; //用于显示sensor输入
    int Sensor_Input;
    public bool isSliderControl = false;
    public PinchSlider pinchSlider; // 用于user输入

    void Start()
    {
        
    }
    void FixedUpdate()
    {
        if (isSliderControl)
        {
            T = pinchSlider.SliderValue;
        }
        else
        {
            if (int.TryParse(Sensor_Data.text, out Sensor_Input))
            {
                //Debug.Log("Extracted number: " + number);
                // 在这里可以将number保存下来或进行其他处理
                Sensor_Input = int.Parse(Sensor_Data.text);
                T = MapIntToFloat(Sensor_Input, 520, 438, 0.4f, 0.8f);
            }
            else
            {
                //Debug.Log("Failed to parse the number.");
            }
        }


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

    float MapIntToFloat(int intValue, int minValue, int maxValue, float minMappedValue, float maxMappedValue)
    {
        float mappedValue = Mathf.InverseLerp(minValue, maxValue, intValue);
        float result = Mathf.Lerp(minMappedValue, maxMappedValue, mappedValue);
        return result;
    }
}