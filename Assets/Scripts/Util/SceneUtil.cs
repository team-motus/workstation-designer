using UnityEngine;
using UnityEngine.InputSystem;
using WorkstationDesigner.InputUtil;

namespace WorkstationDesigner.Util
{
    public static class SceneUtil
	{
		/// <summary>
		/// Calculate the distance from a position to the screen plane
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public static float DistanceToScreenPlane(Vector3 position)
        {
			var offsetFromCamera = (Camera.main.transform.position - position);
			return Vector3.Project(offsetFromCamera, Camera.main.transform.forward).magnitude;
		}

		/// <summary>
		/// Get the cursor positon in the world
		/// </summary>
		/// <param name="requireNotOnUI">Set this to true if the cursor cannot be on top of the UI</param>
		/// <param name="roundCoordinates">Set this to true to round the results to integer values</param>
		/// <param name="requireOnGrid">Set this to true to require the cursor to be over the Grid object</param>
		/// <returns></returns>
		public static Vector3? GetCursorInWorld(bool requireNotOnUI = true, bool roundCoordinates = false, bool requireOnGrid = false)
		{
			// Make sure the mouse isn't over the UI
			if (requireNotOnUI && !MouseManager.GetMouseOver()) { return null; }

			Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

			// Make sure the raycast hit something
			if (!Physics.Raycast(ray, out RaycastHit hit)) { return null; }

			// Make sure the raycast hit the floor
			if (requireOnGrid && hit.collider.gameObject != GameObject.Find("Grid")) { return null; }

			Vector3 point = hit.point;

			if (roundCoordinates)
			{
				// Snap to grid
				point.x = Mathf.Round(point.x);
				point.z = Mathf.Round(point.z);
			}

			return point;
		}
	}
}
