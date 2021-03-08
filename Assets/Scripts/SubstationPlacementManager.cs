using System;
using UnityEngine;
using UnityEngine.InputSystem;
using WorkstationDesigner.InputUtil;
using WorkstationDesigner.Substations;
using WorkstationDesigner.Util;
using WorkstationDesigner.Workstation;

namespace WorkstationDesigner
{
	/// <summary>
	/// Manages the placement of substations in the workstation.
	/// </summary>
	public class SubstationPlacementManager : MonoBehaviour
	{
		public static GameObject WorkstationParent { get; private set; } = null;

		/// <summary>
		/// Represents the currently held substation and where it was picked up from
		/// </summary>
		private class HeldSubstation
		{
			/// <summary>
			/// The currently held substation GameObject
			/// </summary>
			public GameObject SubstationGameObject { get; private set; }

			/// <summary>
			/// The position information for where the held substation was picked up from
			/// </summary>
			private readonly TransformData initialTransformState;

			/// <summary>
			/// Cancel the movement and place the held substation
			/// </summary>
			public Action EscapeAction { get; private set; }

			/// <summary>
			/// A substation object to pick up and its initial transform state
			/// </summary>
			/// <param name="substationGameObject">The substation GameObject</param>
			/// <param name="initialTransformState">Either the substation's transform if it was picked up or null if was just created</param>
			public HeldSubstation(GameObject substationGameObject, Transform initialTransformState)
			{
				this.SubstationGameObject = substationGameObject;
				this.initialTransformState = initialTransformState != null ? TransformData.FromTransform(initialTransformState) : null;

				EscapeAction = () =>
				{
					// Either return to its original position or delete it if it was just created
					if (this.initialTransformState != null)
					{
						this.initialTransformState.SetTransform(this.SubstationGameObject.transform);
						Instance.PlaceSubstation();
					}
					else
					{
						Instance.PlaceSubstation();
						Destroy(this.SubstationGameObject);
					}
				};
			}

			public HeldSubstation() : this(null, null) { }
		}

		private HeldSubstation heldSubstation = null;
		public static SubstationPlacementManager Instance = null;

		public void Awake()
		{
			if (WorkstationParent == null)
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
			if (Instance == this)
			{
				Instance = null;
			}
		}

		public void Update()
		{
			// Check if we should place the held substation
			if (this.heldSubstation != null &&
				!this.heldSubstation.SubstationGameObject.GetComponent<SubstationComponent>().IsIntersecting &&
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
				this.heldSubstation.SubstationGameObject.GetComponent<SubstationComponent>().SetPlaced(true);
				EscManager.PopEscAction(this.heldSubstation.EscapeAction);
				this.heldSubstation = null;
			}
			else
			{
				throw new System.Exception("Cannot place substation (none held)");
			}
		}

		public void PickUpSubstation(GameObject gameObject)
		{
			PickUpSubstation(gameObject, gameObject.transform);
		}

		private void PickUpSubstation(GameObject gameObject, Transform initialTransformState)
		{
			if (this.heldSubstation == null)
			{
				this.heldSubstation = new HeldSubstation(gameObject, initialTransformState);
				EscManager.PushEscAction(this.heldSubstation.EscapeAction);

				SubstationComponent objectComponent = this.heldSubstation.SubstationGameObject.GetComponent<SubstationComponent>();

				objectComponent.SetPlaced(false);
			}
			else
			{
				throw new System.Exception("Cannot pick up a second substation");
			}
		}

		public void CreateSubstation(SubstationBase substation)
		{
			// Destroy currently held substation
			if (this.heldSubstation != null)
			{
				Destroy(this.heldSubstation.SubstationGameObject);
				this.heldSubstation = null;
			}

			// Create new object
			var newObject = substation.Instantiate();
			newObject.GetComponent<Renderer>().enabled = false;
			newObject.transform.parent = WorkstationParent.transform;
			newObject.AddComponent<SubstationComponent>().Substation = substation;

			PickUpSubstation(newObject, null);
		}

		public static Vector3? GetPlacementPoint()
		{
			return SceneUtil.GetCursorInWorld(true, true, true);
		}
	}
}