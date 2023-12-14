using System.IO;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// used to read the txt file
/// </summary>
public class CubePositionReader : MonoBehaviour
{
    public string filename = "CubePositions.txt";

    private Dictionary<string, List<Vector3>> cubePositions = new Dictionary<string, List<Vector3>>();

    void Start()
    {
        ReadCubePositions();
    }

    void ReadCubePositions()
    {
        string[] lines = File.ReadAllLines(filename);
        foreach (string line in lines)
        {
            string[] parts = line.Split(new[] { ": (", "; " }, System.StringSplitOptions.None);
            if (parts.Length == 4)
            {
                string cubeName = parts[1].Trim()+ parts[2].Trim();
                Vector3 position = StringToVector3(parts[parts.Length - 1].Trim(')') );
                if (!cubePositions.ContainsKey(cubeName))
                {
                    cubePositions[cubeName] = new List<Vector3>();
                }
                cubePositions[cubeName].Add(position);
            }
        }
    }

    Vector3 StringToVector3(string s)
    {
        //s = s.Substring(1, s.Length - 2);
        string[] parts = s.Split(',');
        if (parts.Length == 3)
        {
            float x = float.Parse(parts[0]);
            float y = float.Parse(parts[1]);
            float z = float.Parse(parts[2]);
            return new Vector3(x, y, z);
        }
        return Vector3.zero;
    }

    public Vector3 GetCubePosition(string cubeName, float t)
    {
        if (cubePositions.ContainsKey(cubeName))
        {
            List<Vector3> positions = cubePositions[cubeName];
            int index = (int)(t * (positions.Count - 1)); // denote as the location of index
            return positions[index];
        }
        return Vector3.zero;
    }

}