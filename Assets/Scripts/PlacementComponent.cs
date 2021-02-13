using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner
{
    /// <summary>
    /// A temporary version of a workstation component that is displayed when placing the component in the workspace.
    /// </summary>
    public class PlacementComponent : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
			Vector3 hitPoint = -Vector3.one;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// Emit raycast, and collect data on it's termination.
			// If raycast hit 'nothing' default all cordinate values to -1.
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
                hitPoint = hit.point;
                hitPoint.y += this.transform.localScale.y / 2;
                this.transform.position = hitPoint;
                this.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                this.GetComponent<Renderer>().enabled = false;
            }
		}
    }
}
