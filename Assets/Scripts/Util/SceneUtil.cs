using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using WorkstationDesigner.InputUtil;

namespace WorkstationDesigner.Util
{
    public static class SceneUtil
    {
		public static Vector3? GetCursorInWorld(bool roundCoordinates = false, bool requireOnGrid = false)
		{
			// Make sure the mouse isn't over the UI
			if (!MouseManager.GetMouseOver()) { return null; }

			Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

			// Make sure the raycast hit something
			if (!Physics.Raycast(ray, out RaycastHit hit)) { return null; }

			if (requireOnGrid)
			{
				// Make sure the raycast hit the floor
				if (hit.collider.gameObject != GameObject.Find("Grid")) { return null; }
			}

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
