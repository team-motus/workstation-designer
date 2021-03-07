using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using WorkstationDesigner.InputUtil;
using WorkstationDesigner.UI;
using WorkstationDesigner.Util;

namespace WorkstationDesigner.Tools
{
    public class TapeMeasure : MonoBehaviour
    {
        private static TapeMeasure Instance = null;

        /// <summary>
        /// Controls whether the tape measure tool is active or not
        /// </summary>
        public static bool UsingTapeMeasure = false;

        private bool lastUsingTapeMeasure = false;

        // These are set in the Unity Editor for this component
        public GameObject pointPrefab;
        public Texture2D cursorTexture;

        // Point objects
        private GameObject firstPointObject = null;
        private GameObject currentPointObject = null;

        // Scalars to make tape measure look good
        private const float POINT_SIZE_SCALAR = 0.02f; // Chosen experimentally
        private const float LINE_WIDTH_SCALAR = 0.01f; // Chosen experimentally

        private const string TapeMeasurePanelPath = "UI/TapeMeasurePanel";
        private static VisualTreeAsset tapeMeasurePanelAsset = null;
        private static VisualElement tapeMeasurePanel = null;

        public void Awake()
        {
            if (Instance != null)
            {
                throw new Exception("Can only have one TapeMeasure instance");
            }
            Instance = this;

            tapeMeasurePanelAsset = Resources.Load<VisualTreeAsset>(TapeMeasurePanelPath);
            if (tapeMeasurePanelAsset == null)
            {
                throw new Exception("tapeMeasurePanelAsset is null");
            }
            tapeMeasurePanel = tapeMeasurePanelAsset.CloneTree();
        }

        public void Update()
        {
            if (UsingTapeMeasure != lastUsingTapeMeasure)
            {
                // Cursor.SetCursor(UsingTapeMeasure ? cursorTexture : null, Vector2.zero, CursorMode.Auto); // TODO this doesn't seem to work
            }

            UpdateTapeMeasure();

            lastUsingTapeMeasure = UsingTapeMeasure;
        }

        private void UpdateTapeMeasure()
        {
            if (UsingTapeMeasure)
            {
                var currentPoint = SceneUtil.GetCursorInWorld(requireNotOnUI: false); // TODO fix this so it must not be on UI

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
                                UpdateLineWidth();
                                firstPointObject.GetComponent<LineRenderer>().SetPosition(0, firstPointObject.transform.position);
                            }
                        }
                    }
                    else
                    {
                        UpdateLineWidth();
                        firstPointObject.GetComponent<LineRenderer>().SetPosition(1, currentPoint.Value);
                        OpenPanel();
                    }

                    UpdatePoint(firstPointObject);
                    UpdatePoint(currentPointObject);
                    UpdatePanel(firstPointObject, currentPointObject);
                }
                else
                {
                    CleanupPointObject(currentPointObject);
                    if (firstPointObject != null)
                    {
                        firstPointObject.GetComponent<LineRenderer>().SetPosition(1, firstPointObject.transform.position);
                    }
                    ClosePanel();
                }
            }
            else
            {
                CleanupPointObject(firstPointObject);
                CleanupPointObject(currentPointObject);
                ClosePanel();
            }
        }

        private void UpdateLineWidth()
        {
            if (firstPointObject != null)
            {
                var lineRenderer = firstPointObject.GetComponent<LineRenderer>();
                var lineWidth = SceneUtil.DistanceToScreenPlane(firstPointObject.transform) * LINE_WIDTH_SCALAR;

                lineRenderer.startWidth = lineWidth;
                lineRenderer.endWidth = lineWidth;

                if(currentPointObject != null)
                {
                    firstPointObject.GetComponent<LineRenderer>().endWidth = SceneUtil.DistanceToScreenPlane(currentPointObject.transform) * LINE_WIDTH_SCALAR;
                }
            }
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
                point.transform.localScale = Vector3.one * SceneUtil.DistanceToScreenPlane(point.transform) * POINT_SIZE_SCALAR;
            }
        }

        private static void CleanupPointObject(GameObject point)
        {
            if (point != null)
            {
                Destroy(point);
            }
        }

        private static void OpenPanel()
        {
            ScreenManager.OverallContainer.Add(tapeMeasurePanel);
        }

        private static void ClosePanel()
        {
            tapeMeasurePanel.RemoveFromHierarchy();
        }

        private static void UpdatePanel(GameObject first, GameObject current)
        {
            if (first == null || current == null)
            {
                ClosePanel();
                return;
            }

            var distanceVector = current.transform.position - first.transform.position;
            var distance = distanceVector.magnitude;

            var feet = Math.Floor(distance);
            var inches = Math.Round((distance - feet) * 12, 2);

            var cameraCurrentPoint = Camera.main.WorldToScreenPoint(current.transform.position);
            var cameraFirstPoint = Camera.main.WorldToScreenPoint(first.transform.position);
            var cameraDistanceVector = cameraCurrentPoint - cameraFirstPoint;
            var halfPoint = cameraFirstPoint + (cameraDistanceVector * 0.5f);

            var position = halfPoint;

            var panel = tapeMeasurePanel.Q("tape-measure-panel");
            panel.style.top = (Screen.height - position.y - (panel.resolvedStyle.height / 2)) * ScreenManager.dpiScaler; // Flip y position so 0 is at top
            panel.style.left = (position.x - (panel.resolvedStyle.width / 2)) * ScreenManager.dpiScaler;

            /*
            var position = Mouse.current.position.ReadValue();

            const float MOUSE_OFFSET = 10; // px

            var panel = tapeMeasurePanel.Q("tape-measure-panel");
            panel.style.top = (Screen.height - position.y + MOUSE_OFFSET) * ScreenManager.dpiScaler; // Flip y position so 0 is at top
            panel.style.left = (position.x + MOUSE_OFFSET) * ScreenManager.dpiScaler;
            */

            var label = panel.Q<Label>("tape-measure-panel-label");
            label.text = $"{feet}' {inches}\"";
        }
    }
}
