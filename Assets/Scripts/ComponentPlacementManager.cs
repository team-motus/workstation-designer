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
		private GameObject PlacementComponentObject;

		public ComponentPlacementManager()
		{
			this.ActiveComponent = null;
		}

		void Start()
		{

		}

		void Update()
		{
			if (Input.GetMouseButtonDown(0) && this.ActiveComponent != null && this.PlacementComponentObject != null && !this.PlacementComponentObject.GetComponent<PlacementComponent>().IsIntersecting)
			{
				Vector3? maybePlacePoint = ComponentPlacementManager.GetPlacementPoint(this.ActiveComponent);
				if (maybePlacePoint.HasValue)
                {
					Vector3 placePoint = maybePlacePoint.Value;

					Destroy(this.PlacementComponentObject);
					this.PlacementComponentObject = null;

					GameObject componentObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
					componentObject.GetComponent<BoxCollider>().size = new Vector3((float) 0.99, 1, (float) 0.99);
					componentObject.GetComponent<BoxCollider>().isTrigger = true;
					PlacedComponent placedComponent = componentObject.AddComponent<PlacedComponent>();
					placedComponent.component = this.ActiveComponent;

					componentObject.transform.localScale = new Vector3(this.ActiveComponent.FootprintDimensions.Item1, 6, this.ActiveComponent.FootprintDimensions.Item2);

					placePoint.y += componentObject.transform.localScale.y / 2;
					componentObject.transform.position = placePoint;

					this.ActiveComponent = null;
                }
			}
			else if (this.PlacementComponentObject != null && this.PlacementComponentObject.GetComponent<PlacementComponent>().IsIntersecting)
			{
				//TODO: Indicate this to the user
			}
		}

		public void ActivateComponent(ComponentModel component)
		{
			this.ActiveComponent = component;

			if (this.PlacementComponentObject != null)
			{
				Destroy(this.PlacementComponentObject);
			}
			this.PlacementComponentObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			this.PlacementComponentObject.GetComponent<Renderer>().enabled = false;
			this.PlacementComponentObject.GetComponent<BoxCollider>().size = new Vector3((float)0.99, 1, (float)0.99);
			this.PlacementComponentObject.GetComponent<BoxCollider>().isTrigger = true;
			this.PlacementComponentObject.AddComponent<Rigidbody>().isKinematic = false; // Attach a non-kinematic rigidbody to enable collision detection
			this.PlacementComponentObject.layer = 2; // Ignore raycast
			PlacementComponent placementComponent = this.PlacementComponentObject.AddComponent<PlacementComponent>();
			placementComponent.Component = this.ActiveComponent;
			this.PlacementComponentObject.transform.localScale = new Vector3(component.FootprintDimensions.Item1, 6, component.FootprintDimensions.Item2);
		}

		public static Vector3? GetPlacementPoint(ComponentModel component)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

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