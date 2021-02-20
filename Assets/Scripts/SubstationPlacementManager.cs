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

		void Update()
		{
			if (this.PlacementSubstationObject != null)
			{
				if (this.PlacementSubstationObject.GetComponent<PlacementSubstation>().IsIntersecting)
				{
					//TODO: Indicate this to the user
				}
				else if (Input.GetMouseButtonDown(0) && this.ActiveSubstation != null)
				{
					Vector3? maybePlacePoint = SubstationPlacementManager.GetPlacementPoint(this.ActiveSubstation);
					if (maybePlacePoint.HasValue)
					{
						// Replace PlacementSubstation with PlacedSubstation
						this.PlacementSubstationObject.AddComponent<PlacedSubstation>();
						Destroy(this.PlacementSubstationObject.GetComponent<PlacementSubstation>());

						// Reset SubstationPlacementManager
						this.PlacementSubstationObject = null;
						this.ActiveSubstation = null;
					}
				}
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

			var rigidBody = this.PlacementSubstationObject.AddComponent<Rigidbody>();
			rigidBody.isKinematic = false; // Attach a non-kinematic rigidbody to enable collision detection
			rigidBody.useGravity = false;

			this.PlacementSubstationObject.layer = 2; // Ignore raycast

			PlacementSubstation placementSubstation = this.PlacementSubstationObject.AddComponent<PlacementSubstation>();
			placementSubstation.Substation = this.ActiveSubstation;

			this.PlacementSubstationObject.transform.localScale = new Vector3(substation.FootprintDimensions.Item1, 6, substation.FootprintDimensions.Item2);
		}

		public static Vector3? GetPlacementPoint(SubstationModel substation)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// Make sure the mouse isn't over the UI
			if (!MouseManager.GetMouseOver()) { return null; }

			// Make sure the raycast hit something
			if (!Physics.Raycast(ray, out RaycastHit hit)) { return null; }

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