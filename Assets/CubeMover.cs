using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;

public class CubeMover : MonoBehaviour
{
    // ����һ���������洢��һ���ű���ʵ��
    public CubeSpacePointsFitting cubeSpacePointsFitting;
    // ����һ����Ϸ�����������洢��Ҫ����λ�õ�cube
    public GameObject[] cubes;
    public float T = 0.5f;
    List<Vector3> trajectoryPoints;
    public TextMeshProUGUI Sensor_Data; //������ʾsensor����
    int Sensor_Input;
    public bool isSliderControl = false;
    public PinchSlider pinchSlider; // ����user����

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
                // ��������Խ�number���������������������
                Sensor_Input = int.Parse(Sensor_Data.text);
                T = MapIntToFloat(Sensor_Input, 520, 438, 0.4f, 0.8f);
            }
            else
            {
                //Debug.Log("Failed to parse the number.");
            }
        }


        T = Mathf.Clamp(T, 0f, 1f);
        // ������һ���ű��ķ�����ȡ�켣���б�
        trajectoryPoints = cubeSpacePointsFitting.GetTrajectoryPoint_CUBE(T);

        // ����ÿ��cube��λ��
        for (int i = 0; i < cubes.Length; i++)
        {
            // ���켣���б�������Ƿ񳬳���Χ
            if (i < trajectoryPoints.Count)
            {
                // ����cube�ľֲ�λ��Ϊ��Ӧ�Ĺ켣��
                cubes[i].transform.localPosition = trajectoryPoints[i];
            }
            else
            {
                // ����켣���б������������Χ����cube�ľֲ�λ�ò���
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