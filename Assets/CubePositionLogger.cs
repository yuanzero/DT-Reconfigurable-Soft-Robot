using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CubePositionLogger: MonoBehaviour
{
    public CubeGroup[] cubeGroups;
    public Button saveButton;
    int cout = 0;

    private void Start()
    {
        // ���ð�ť�ĵ���¼�
        saveButton.onClick.AddListener(SaveCubePositions);

        // �ڱ���ʱ����������ļ�
        File.WriteAllText("CubePositions.txt", string.Empty);
    }

    public void SaveCubePositions()
    {
        using (StreamWriter writer = new StreamWriter("CubePositions.txt", true))
        {
            cout += 1;
            for (int i = 0; i < cubeGroups.Length; i++)
            {
                for (int j = 0; j < cubeGroups[i].cubes.Length; j++)
                {
                    Vector3 position = cubeGroups[i].cubes[j].transform.localPosition;
                    writer.WriteLine($"Cube group {i}; Cout {cout}; cube {j}: {position}");
                    Debug.Log($"Cube group {i},Cout {cout}, cube {j}: {position}");
                }
            }
        }
    }
}