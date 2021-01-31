using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  This script manages mouse input to provide information and allow manipulation of the objects in a scene.
 *  By default the only output of this script is a console log with coordinates of the object that was clicked, but can be revised to perform any number of manipulations to the environment.
 */ 

public class ClickManager : MonoBehaviour{

	// Start is called before the first frame update
	void Update(){

		// if mouse input is detected (specifically a left-click)
		if (Input.GetMouseButtonDown(0)){

			// set the position of a vector to originate from the main screen camera.
			Vector3 clickPosition = -Vector3.one;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// emit raycast, and colect data on it's termination.
			RaycastHit hit;

			// if raycast hit 'nothing' default all cordinate values to -1.
			if (Physics.Raycast(ray, out hit)){
				clickPosition = hit.point;
			}

			// report raycast coordinates.
			Debug.Log(clickPosition);
		}
	}
}
