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
				// set the position of a vector to originate from the main screen camera.
				Vector3 clickPosition = -Vector3.one;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				// Emit raycast, and colect data on it's termination.
				// If raycast hit 'nothing' default all cordinate values to -1.
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))
				{
					clickPosition = hit.point;
				}

				// Report raycast coordinates and active component.
				Debug.Log(clickPosition);
				Debug.Log(this.ActiveComponent.Name);

				Destroy(this.PlacementComponent);
				this.PlacementComponent = null;

				GameObject componentObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
				componentObject.AddComponent<PlacedComponent>();
				componentObject.transform.localScale = new Vector3(ActiveComponent.FootprintDimensions.Item1, 6, ActiveComponent.FootprintDimensions.Item2);
				clickPosition.y += componentObject.transform.localScale.y / 2;
				componentObject.transform.position = clickPosition;

				this.ActiveComponent = null;
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
			this.PlacementComponent.layer = 2; // Ignore raycast
			this.PlacementComponent.AddComponent<PlacementComponent>();
			this.PlacementComponent.transform.localScale = new Vector3(component.FootprintDimensions.Item1, 6, component.FootprintDimensions.Item2);
		}
	}
}