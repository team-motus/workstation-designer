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
		public static GameObject WorkstationParent { get; private set; } = null;

		private GameObject heldSubstation = null;
		public static SubstationPlacementManager Instance = null;

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
			// Check if we should place the held substation
			if (this.heldSubstation != null &&
				!this.heldSubstation.GetComponent<SubstationComponent>().IsIntersecting &&
				GetPlacementPoint().HasValue &&
				Mouse.current.leftButton.wasPressedThisFrame)
			{
				PlaceSubstation();
			}
		}

		public void PlaceSubstation()
        {
			if (this.heldSubstation != null)
			{
				this.heldSubstation.GetComponent<SubstationComponent>().SetPlaced(true);
				this.heldSubstation = null;
			}
			else
			{
				throw new System.Exception("Cannot place substation (none held)");
			}
		}

		public void PickUpSubstation(GameObject gameObject)
        {
			if (this.heldSubstation == null)
			{
				this.heldSubstation = gameObject;

				SubstationComponent objectComponent = this.heldSubstation.GetComponent<SubstationComponent>();

				objectComponent.SetPlaced(false);
			} else
            {
				throw new System.Exception("Cannot pick up a second substation");
            }
		}

		public void CreateSubstation(SubstationBase substation)
		{
			// Destroy currently held substation
			if (this.heldSubstation != null)
			{
				Destroy(this.heldSubstation);
				this.heldSubstation = null;
			}

			// Create new object
			var newObject = substation.Instantiate();
			newObject.GetComponent<Renderer>().enabled = false;
			newObject.transform.parent = WorkstationParent.transform;
			newObject.AddComponent<SubstationComponent>().Substation = substation;

			PickUpSubstation(newObject);
		}

		public static Vector3? GetPlacementPoint()
		{
			return SceneUtil.GetCursorInWorld(true, true, true);
		}
	}
}