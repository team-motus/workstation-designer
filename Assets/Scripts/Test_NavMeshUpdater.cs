using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AI;
using UnityEngine.AI;

public class Test_NavMeshUpdater : MonoBehaviour
{
    //public NavMeshSurface surface;
    // Start is called before the first frame update
    void Start()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(7.5f, 4, 6);
        cube.transform.localScale = new Vector3(2, 2, 2);
        //NavMeshBuilder.BuildNavMesh();

        //surface.BuildNavMesh();
    }
}
