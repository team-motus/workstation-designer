using UnityEngine;
using UnityEngine.InputSystem;
using WorkstationDesigner.InputUtil;

namespace WorkstationDesigner.Util
{
    public static class SceneUtil
	{
		public static float DistanceToScreenPlane(Transform transform)
        {
			var offsetFromCamera = (Camera.main.transform.position - transform.position);
			return Vector3.Project(offsetFromCamera, Camera.main.transform.forward).magnitude;
		}

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
