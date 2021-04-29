using System;
using UnityEngine;
using UnityEngine.InputSystem;
using WorkstationDesigner.InputUtil;
using WorkstationDesigner.Workstation.Substations;
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
			/// A callback to perform when the held substation is placed
			/// </summary>
			public Action PlaceCallback { get; private set; }

			/// <summary>
			/// A substation object to pick up and its initial transform state
			/// </summary>
			/// <param name="substationGameObject">The substation GameObject</param>
			/// <param name="initialTransformState">Either the substation's transform if it was picked up or null if was just created</param>
			/// <param name="placeCallback">A callback to perform when the held substation is placed</param>
			public HeldSubstation(GameObject substationGameObject, Transform initialTransformState, Action placeCallback)
			{
				this.SubstationGameObject = substationGameObject;
				this.initialTransformState = initialTransformState != null ? TransformData.FromTransform(initialTransformState) : null;
				this.PlaceCallback = placeCallback;

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

			public HeldSubstation() : this(null, null, null) { }
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

		/// <summary>
		/// Place the currently held substation
		/// </summary>
		public void PlaceSubstation()
		{
			if (this.heldSubstation != null)
			{
				this.heldSubstation.SubstationGameObject.GetComponent<SubstationComponent>().SetPlaced(true);
				EscManager.PopEscAction(this.heldSubstation.EscapeAction);

				if (this.heldSubstation.PlaceCallback != null)
				{
					this.heldSubstation.PlaceCallback();
				}

				this.heldSubstation = null;
			}
			else
			{
				throw new System.Exception("Cannot place substation (none held)");
			}
		}

		/// <summary>
		/// Pick up a substation GameObject
		/// </summary>
		/// <param name="gameObject"></param>
		public void PickUpSubstation(GameObject gameObject)
		{
			PickUpSubstation(new HeldSubstation(gameObject, gameObject.transform, null));
		}

		/// <summary>
		/// Pick up a substation represented as a HeldSubstation
		/// </summary>
		/// <param name="newHeldSubstation"></param>
		private void PickUpSubstation(HeldSubstation newHeldSubstation)
		{
			if (this.heldSubstation == null)
			{
				this.heldSubstation = newHeldSubstation;
				EscManager.PushEscAction(this.heldSubstation.EscapeAction);

				SubstationComponent objectComponent = this.heldSubstation.SubstationGameObject.GetComponent<SubstationComponent>();

				objectComponent.SetPlaced(false);
			}
			else
			{
				throw new Exception("Cannot pick up a second substation");
			}
		}

		/// <summary>
		/// Create a new substation to place in the scene
		/// </summary>
		/// <param name="substation"></param>
		/// <param name="placeCallback"></param>
		public void CreateSubstation(SubstationBase substation, Action placeCallback)
		{
			// Destroy currently held substation
			if (this.heldSubstation != null)
			{
				Destroy(this.heldSubstation.SubstationGameObject);
				this.heldSubstation = null;
			}

			// Create new object
			var newObject = substation.Instantiate();
			newObject.transform.parent = WorkstationParent.transform;
			var substationComponent = newObject.AddComponent<SubstationComponent>();
			substationComponent.Substation = substation;
			substationComponent.SetVisible(false);

			PickUpSubstation(new HeldSubstation(newObject, null, () =>
			{
				substationComponent.SetSelected(true);

				Action selectAction = () => substationComponent.SetSelected(false);
				EscManager.PushEscAction(selectAction);
				placeCallback();
			}));
		}

		/// <summary>
		/// Get possible point at which a new substation will be placed
		/// </summary>
		/// <returns></returns>
		public static Vector3? GetPlacementPoint()
		{
			return SceneUtil.GetCursorInWorld(true, true, true);
		}
	}
}
