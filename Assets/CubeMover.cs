using UnityEngine;
using System.Collections.Generic;

public class CubeMover : MonoBehaviour
{
    // ����һ���������洢��һ���ű���ʵ��
    public CubeSpacePointsFitting cubeSpacePointsFitting;
    // ����һ����Ϸ�����������洢��Ҫ����λ�õ�cube
    public GameObject[] cubes;
    public float T = 0.5f;
    List<Vector3> trajectoryPoints;

    void Start()
    {
        
    }
    void FixedUpdate()
    {
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
}