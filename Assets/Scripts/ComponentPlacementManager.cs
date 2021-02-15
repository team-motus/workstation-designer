using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner
{
	/// <summary>
	/// Manages the placement of components in the workstation.
	/// </summary>
	public class ComponentPlacementManager : MonoBehaviour
	{
		private ComponentModel ActiveComponent;
		private GameObject PlacementComponent;

		public ComponentPlacementManager()
		{
			this.ActiveComponent = null;
		}

		void Start()
		{

		}

		void Update()
		{
			if (Input.GetMouseButtonDown(0) && this.ActiveComponent != null)
			{
				Vector3? maybePlacePoint = ComponentPlacementManager.GetPlacementPoint();
				if (maybePlacePoint.HasValue)
                {
					Vector3 placePoint = maybePlacePoint.Value;

					Destroy(this.PlacementComponent);
					this.PlacementComponent = null;

					GameObject componentObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
					componentObject.AddComponent<PlacedComponent>();
					componentObject.transform.localScale = new Vector3(ActiveComponent.FootprintDimensions.Item1, 6, ActiveComponent.FootprintDimensions.Item2);
					placePoint.y += componentObject.transform.localScale.y / 2;
					componentObject.transform.position = placePoint;

					this.ActiveComponent = null;
                }
			}
		}

		public void ActivateComponent(ComponentModel component)
		{
			this.ActiveComponent = component;

			if (this.PlacementComponent != null)
			{
				Destroy(this.PlacementComponent);
			}
			this.PlacementComponent = GameObject.CreatePrimitive(PrimitiveType.Cube);
			this.PlacementComponent.GetComponent<Renderer>().enabled = false;
			this.PlacementComponent.layer = 2; // Ignore raycast
			this.PlacementComponent.AddComponent<PlacementComponent>();
			this.PlacementComponent.transform.localScale = new Vector3(component.FootprintDimensions.Item1, 6, component.FootprintDimensions.Item2);
		}

		public static Vector3? GetPlacementPoint()
        {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			// Make sure the raycast hit something
			if (!Physics.Raycast(ray, out hit))
			{
				return null;
			}

			// Make sure the raycast hit the floor
			if (hit.collider.gameObject != GameObject.Find("Grid"))
			{
				return null;
			}

			Vector3 placePoint = hit.point;

			// Snap to grid
			placePoint.x = Mathf.Round(placePoint.x);
			placePoint.z = Mathf.Round(placePoint.z);

			return placePoint;
		}
	}
}