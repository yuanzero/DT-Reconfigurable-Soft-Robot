using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CubeGroup : MonoBehaviour
{
    public GameObject[] cubes;

    private void Start()
    {
        List<MeshFilter> meshFilters = GetComponentsInChildren<MeshFilter>().ToList();
        cubes = meshFilters.Select(filter => filter.gameObject).ToArray();
    }
}