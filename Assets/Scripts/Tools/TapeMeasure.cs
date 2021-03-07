using System;
using UnityEngine;
using UnityEngine.InputSystem;
using WorkstationDesigner.InputUtil;
using WorkstationDesigner.Util;

namespace WorkstationDesigner.Tools
{
    public class TapeMeasure : MonoBehaviour
    {
        private static TapeMeasure Instance = null;

        public static bool UsingTapeMeasure = false;
        private bool lastUsingTapeMeasure = false;

        public GameObject pointPrefab;
        public Texture2D cursorTexture;

        private GameObject firstPointObject = null;
        private GameObject currentPointObject = null;

        private const float POINT_SCALAR = 0.02f; // Chosen experimentally
        private const float LINE_WIDTH = 0.25f; // Chosen experimentally

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                throw new Exception("Can only have one TapeMeasure instance");
            }
        }

        public void Update()
        {
            if (UsingTapeMeasure != lastUsingTapeMeasure)
            {
                // Cursor.SetCursor(UsingTapeMeasure ? cursorTexture : null, Vector2.zero, CursorMode.Auto); // TODO this doesn't seem to work
            }

            if (UsingTapeMeasure)
            {
                var currentPoint = SceneUtil.GetCursorInWorld();

                if (currentPoint.HasValue)
                {
                    CreatePoint(ref currentPointObject, currentPoint.Value);

                    if (firstPointObject == null)
                    {
                        if (Mouse.current.leftButton.wasPressedThisFrame && MouseManager.GetMouseOver())
                        {
                            var firstPoint = SceneUtil.GetCursorInWorld();

                            if (firstPoint.HasValue)
                            {
                                CreatePoint(ref firstPointObject, firstPoint.Value);
                                var lineRenderer = firstPointObject.GetComponent<LineRenderer>();
                                lineRenderer.startWidth = LINE_WIDTH;
                                lineRenderer.endWidth = LINE_WIDTH;
                                lineRenderer.SetPosition(0, firstPointObject.transform.position);

                            }
                        }
                    }
                    else
                    {
                        firstPointObject.GetComponent<LineRenderer>().SetPosition(1, currentPoint.Value);
                    }
                }
                else
                {
                    CleanupPointObject(currentPointObject);
                    if (firstPointObject != null)
                    {
                        firstPointObject.GetComponent<LineRenderer>().SetPosition(1, firstPointObject.transform.position);
                    }
                }

                UpdatePoint(firstPointObject);
                UpdatePoint(currentPointObject);
            }
            else
            {
                CleanupPointObject(firstPointObject);
                CleanupPointObject(currentPointObject);
            }

            lastUsingTapeMeasure = UsingTapeMeasure;
        }

        private static void CreatePoint(ref GameObject point, Vector3 position)
        {
            if (point == null)
            {
                point = Instantiate(Instance.pointPrefab);
                point.transform.parent = Instance.gameObject.transform;
            }
            point.transform.position = position;
        }

        private static void UpdatePoint(GameObject point)
        {
            if (point != null)
            {
                point.transform.forward = Camera.main.transform.forward;
                var offsetFromCamera = (Camera.main.transform.position - point.transform.position);
                var distanceToCamera = Vector3.Project(offsetFromCamera, Camera.main.transform.forward).magnitude;
                point.transform.localScale = Vector3.one * distanceToCamera * POINT_SCALAR;
            }
        }

        private static void CleanupPointObject(GameObject point)
        {
            if (point != null)
            {
                Destroy(point);
            }
        }
    }
}
