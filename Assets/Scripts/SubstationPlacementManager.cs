using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.InputUtil;

namespace WorkstationDesigner
{
	/// <summary>
	/// Manages the placement of substations in the workstation.
	/// </summary>
	public class SubstationPlacementManager : MonoBehaviour
	{
		private SubstationModel ActiveSubstation;
		private GameObject PlacementSubstationObject;

		public SubstationPlacementManager()
		{
			this.ActiveSubstation = null;
		}

		void Start()
		{

		}

		void Update()
		{
			if (Input.GetMouseButtonDown(0) && this.ActiveSubstation != null && this.PlacementSubstationObject != null && !this.PlacementSubstationObject.GetComponent<PlacementSubstation>().IsIntersecting)
			{
				Vector3? maybePlacePoint = SubstationPlacementManager.GetPlacementPoint(this.ActiveSubstation);
				if (maybePlacePoint.HasValue)
                {
					Vector3 placePoint = maybePlacePoint.Value;

					Destroy(this.PlacementSubstationObject);
					this.PlacementSubstationObject = null;

					GameObject substationObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
					substationObject.GetComponent<BoxCollider>().size = new Vector3((float) 0.99, 1, (float) 0.99);
					substationObject.GetComponent<BoxCollider>().isTrigger = true;
					PlacedSubstation placedSubstation = substationObject.AddComponent<PlacedSubstation>();
					placedSubstation.substation = this.ActiveSubstation;

					substationObject.transform.localScale = new Vector3(this.ActiveSubstation.FootprintDimensions.Item1, 6, this.ActiveSubstation.FootprintDimensions.Item2);

					placePoint.y += substationObject.transform.localScale.y / 2;
					substationObject.transform.position = placePoint;

					this.ActiveSubstation = null;
                }
			}
			else if (this.PlacementSubstationObject != null && this.PlacementSubstationObject.GetComponent<PlacementSubstation>().IsIntersecting)
			{
				//TODO: Indicate this to the user
			}
		}

		public void ActivateSubstation(SubstationModel substation)
		{
			this.ActiveSubstation = substation;

			if (this.PlacementSubstationObject != null)
			{
				Destroy(this.PlacementSubstationObject);
			}
			this.PlacementSubstationObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			this.PlacementSubstationObject.GetComponent<Renderer>().enabled = false;
			this.PlacementSubstationObject.GetComponent<BoxCollider>().size = new Vector3((float)0.99, 1, (float)0.99);
			this.PlacementSubstationObject.GetComponent<BoxCollider>().isTrigger = true;
			this.PlacementSubstationObject.AddComponent<Rigidbody>().isKinematic = false; // Attach a non-kinematic rigidbody to enable collision detection
			this.PlacementSubstationObject.layer = 2; // Ignore raycast
			PlacementSubstation placementSubstation = this.PlacementSubstationObject.AddComponent<PlacementSubstation>();
			placementSubstation.Substation = this.ActiveSubstation;
			this.PlacementSubstationObject.transform.localScale = new Vector3(substation.FootprintDimensions.Item1, 6, substation.FootprintDimensions.Item2);
		}

		public static Vector3? GetPlacementPoint(SubstationModel substation)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			// Make sure the mouse isn't over the UI
			if (!MouseManager.GetMouseOver()) { return null; }

			// Make sure the raycast hit something
			if (!Physics.Raycast(ray, out hit)) { return null; }

			// Make sure the raycast hit the floor
			if (hit.collider.gameObject != GameObject.Find("Grid")) { return null; }

			Vector3 placePoint = hit.point;

			// Snap to grid
			placePoint.x = Mathf.Round(placePoint.x);
			placePoint.z = Mathf.Round(placePoint.z);

			return placePoint;
		}
	}
}