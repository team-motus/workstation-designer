using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner
{
	public class ComponentPlacementManager : MonoBehaviour
	{
		private ComponentModel ActiveComponent;

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

				GameObject componentObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
				clickPosition.y += (float) 0.5;
				componentObject.transform.position = clickPosition;
				componentObject.transform.localScale = new Vector3(ActiveComponent.FootprintDimensions.Item1, 1, ActiveComponent.FootprintDimensions.Item2);

				this.ActiveComponent = null;
			}
		}

		public void ActivateComponent(ComponentModel component)
		{
			this.ActiveComponent = component;
		}
	}
}