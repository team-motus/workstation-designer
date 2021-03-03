using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  This script controls the variable construction status of the brickstack, disabling bricks that have not yet been placed, enabling the bricks that have, and keeping track of the count of each brick.
 *  By modifying the values in this script (namely brickCount) a worker, user, or other function can modify the number of bricks that have been placed, and thus the construction progress of the stack.
 */

public class SampleWall : MonoBehaviour
{
    public int studCount;
    public int plateCount;
    public GameObject[] studList;
    public GameObject[] plateList;

    // Enable/disable individual components of a blueprint based on it's construction progress.
    void UpdateBluprintComponents(int objCount, GameObject[] objList)
    {
        // For each component of it's type in the blueprint
        for (int i = 0; i < objList.Length; i++)
        {
            // if the component has been placed according to the count of bricks
            if (i < objCount)
            {
                // keep renderer enabled.
                objList[i].GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                // disable renderer.
                objList[i].GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBluprintComponents(studCount, studList);
        UpdateBluprintComponents(plateCount, plateList);
    }
}