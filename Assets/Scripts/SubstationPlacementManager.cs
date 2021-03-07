using UnityEngine;
using UnityEngine.InputSystem;
using WorkstationDesigner.InputUtil;
using WorkstationDesigner.Substations;
using WorkstationDesigner.Util;

namespace WorkstationDesigner
{
	/// <summary>
	/// Manages the placement of substations in the workstation.
	/// </summary>
	public class SubstationPlacementManager : MonoBehaviour
	{
		public static GameObject WorkstationParent = null;

		private SubstationBase ActiveSubstation;
		private GameObject PlacementSubstationObject;
		public static SubstationPlacementManager Instance = null;

		public SubstationPlacementManager()
		{
			this.ActiveSubstation = null;
		}

        public void Awake()
        {
			if(WorkstationParent == null)
            {
				WorkstationParent = GameObject.Find("WorkstationParent");
				if (WorkstationParent == null)
				{
					throw new System.Exception("WorkstationParent is null");
				}
			}

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

        public void Update()
		{
			if (this.PlacementSubstationObject != null)
			{
				if (this.PlacementSubstationObject.GetComponent<PlacementSubstation>().IsIntersecting)
				{
					//TODO: Indicate this to the user
				}
				else if (Mouse.current.leftButton.wasPressedThisFrame && this.ActiveSubstation != null)
				{
					Vector3? maybePlacePoint = GetPlacementPoint();
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
				Destroy(obj.GetComponent<PlacementSubstation>());

				// Reset SubstationPlacementManager
				this.ActiveSubstation = null;
				this.PlacementSubstationObject = null;
			}
		}

		public void ActivateSubstation(SubstationBase substation)
		{
			this.ActiveSubstation = substation;

			if (this.PlacementSubstationObject != null)
			{
				Destroy(this.PlacementSubstationObject);
			}

			this.PlacementSubstationObject = this.ActiveSubstation.Instantiate();
			this.PlacementSubstationObject.GetComponent<Renderer>().enabled = false;
			this.PlacementSubstationObject.transform.parent = WorkstationParent.transform;

			PlacementSubstation placementSubstation = this.PlacementSubstationObject.AddComponent<PlacementSubstation>();
			placementSubstation.Substation = this.ActiveSubstation;
		}

		public static Vector3? GetPlacementPoint()
		{
			return SceneUtil.GetCursorInWorld(true, true);
		}
	}
}