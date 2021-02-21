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
		public static SubstationPlacementManager Instance = null;

		public SubstationPlacementManager()
		{
			this.ActiveSubstation = null;
		}

        public void Awake()
        {
			if (Instance == null)
			{
				Instance = this;

			}
			else
			{
				throw new System.Exception("Can only be one instance of SubstationPlacementManager");
			}
		}

        public void OnDestroy()
        {
            if(Instance == this)
            {
				Instance = null;
			}
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
						MakePlacedSubstation(this.PlacementSubstationObject);
					}
				}
			}
		}

		/// <summary>
		/// Replace PlacedSubstation with PlacementSubstation
		/// </summary>
		/// <param name="obj"></param>
		public void MakePlacementSubstation(GameObject obj)
        {
			if (obj.GetComponent<PlacedSubstation>() != null)
			{
				obj.AddComponent<PlacementSubstation>().Substation = obj.GetComponent<PlacedSubstation>().Substation;
				obj.layer = 2; // Ignore raycast
				Destroy(obj.GetComponent<PlacedSubstation>());

				// Set SubstationPlacementManager
				this.ActiveSubstation = obj.GetComponent<PlacementSubstation>().Substation;
				this.PlacementSubstationObject = obj;
			}
		}

		/// <summary>
		/// Replace PlacementSubstation with PlacedSubstation
		/// </summary>
		/// <param name="obj"></param>
		public void MakePlacedSubstation(GameObject obj)
		{
			if (obj.GetComponent<PlacementSubstation>() != null)
			{
				obj.AddComponent<PlacedSubstation>().Substation = obj.GetComponent<PlacementSubstation>().Substation;
				obj.layer = 0; // Default
				Destroy(obj.GetComponent<PlacementSubstation>());

				// Reset SubstationPlacementManager
				this.ActiveSubstation = null;
				this.PlacementSubstationObject = null;
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