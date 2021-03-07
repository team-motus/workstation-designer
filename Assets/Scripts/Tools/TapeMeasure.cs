using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using WorkstationDesigner.InputUtil;
using WorkstationDesigner.UI;
using WorkstationDesigner.Util;

namespace WorkstationDesigner.Tools
{
    /// <summary>
    /// This class manages every part of the tape measure tool
    /// </summary>
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

        // Tape measure label
        private const string TapeMeasurePanelPath = "UI/TapeMeasurePanel";
        private static VisualTreeAsset tapeMeasurePanelAsset = null;
        private static VisualElement tapeMeasurePanel = null;

        private static Action EscAction = () => { UsingTapeMeasure = false; };

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
                if (UsingTapeMeasure)
                {
                    EscManager.PushEscAction(EscAction);
                }
                else
                {
                    EscManager.PopEscAction(EscAction);
                }

                // TODO this doesn't seem to work
                // Set the cursor to a tape measure icon when active
                // UnityEngine.Cursor.SetCursor(UsingTapeMeasure ? cursorTexture : null, Vector2.zero, CursorMode.Auto);
            }

            UpdateTapeMeasure();

            lastUsingTapeMeasure = UsingTapeMeasure;
        }

        /// <summary>
        /// Update all parts related to the tape measure tool
        /// </summary>
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
                        // Set up the first point on click if it doesn't exist yet
                        if (Mouse.current.leftButton.wasPressedThisFrame && MouseManager.GetMouseOver())
                        {
                            var firstPoint = SceneUtil.GetCursorInWorld();

                            if (firstPoint.HasValue)
                            {
                                CreatePoint(ref firstPointObject, firstPoint.Value);

                                UpdateLineWidth();

                                // Set first position of tape measure line to the be the position of the first point
                                firstPointObject.GetComponent<LineRenderer>().SetPosition(0, firstPointObject.transform.position);
                            }
                        }
                    }
                    else
                    {
                        UpdateLineWidth();

                        // Set second position (endpoint) of tape measure line to be at the current position
                        firstPointObject.GetComponent<LineRenderer>().SetPosition(1, currentPoint.Value);

                        OpenPanel();
                    }

                    // Update point appearance and panel value
                    UpdatePoint(firstPointObject);
                    UpdatePoint(currentPointObject);
                    UpdatePanel(firstPointObject, currentPointObject);
                }
                else
                {
                    // Only clean up current point and panel (i.e. leave first point alone)
                    CleanupPointObject(currentPointObject);
                    if (firstPointObject != null)
                    {
                        // Reset the line endpoint
                        firstPointObject.GetComponent<LineRenderer>().SetPosition(1, firstPointObject.transform.position);
                    }
                    ClosePanel();
                }
            }
            else
            {
                // Clean up everything
                CleanupPointObject(firstPointObject);
                CleanupPointObject(currentPointObject);
                ClosePanel();
            }
        }

        /// <summary>
        /// Update line width to be constant regardless of distance to camera plane
        /// </summary>
        private void UpdateLineWidth()
        {
            if (firstPointObject != null)
            {
                var lineRenderer = firstPointObject.GetComponent<LineRenderer>();
                var lineWidth = SceneUtil.DistanceToScreenPlane(firstPointObject.transform.position) * LINE_WIDTH_SCALAR;

                lineRenderer.startWidth = lineWidth;
                lineRenderer.endWidth = lineWidth;

                if(currentPointObject != null)
                {
                    firstPointObject.GetComponent<LineRenderer>().endWidth = SceneUtil.DistanceToScreenPlane(currentPointObject.transform.position) * LINE_WIDTH_SCALAR;
                }
            }
        }

        /// <summary>
        /// Create a point marker at a position
        /// </summary>
        /// <param name="point"></param>
        /// <param name="position"></param>
        private static void CreatePoint(ref GameObject point, Vector3 position)
        {
            if (point == null)
            {
                point = Instantiate(Instance.pointPrefab);
                point.transform.parent = Instance.gameObject.transform;
            }
            point.transform.position = position;
        }

        /// <summary>
        /// Update a point's appearance to always face the camera and to remain at a constant size regardless of distance to camera plane
        /// </summary>
        /// <param name="point"></param>
        private static void UpdatePoint(GameObject point)
        {
            if (point != null)
            {
                point.transform.forward = Camera.main.transform.forward;
                point.transform.localScale = Vector3.one * SceneUtil.DistanceToScreenPlane(point.transform.position) * POINT_SIZE_SCALAR;
            }
        }

        /// <summary>
        /// Destroy a point marker
        /// </summary>
        /// <param name="point"></param>
        private static void CleanupPointObject(GameObject point)
        {
            if (point != null)
            {
                Destroy(point);
            }
        }

        /// <summary>
        /// Open the label panel
        /// </summary>
        private static void OpenPanel()
        {
            ScreenManager.OverallContainer.Add(tapeMeasurePanel);
        }

        /// <summary>
        /// Close the label panel
        /// </summary>
        private static void ClosePanel()
        {
            tapeMeasurePanel.RemoveFromHierarchy();
        }

        /// <summary>
        /// Update the label panel
        /// </summary>
        /// <param name="first">The first point in world space</param>
        /// <param name="current">The second point in world space</param>
        private static void UpdatePanel(GameObject first, GameObject current)
        {
            if (first == null || current == null)
            {
                ClosePanel();
                return;
            }

            // Calculate the half way point between the two point markers in camera space
            var cameraCurrentPoint = Camera.main.WorldToScreenPoint(current.transform.position);
            var cameraFirstPoint = Camera.main.WorldToScreenPoint(first.transform.position);
            var cameraDistanceVector = cameraCurrentPoint - cameraFirstPoint;
            var halfPoint = cameraFirstPoint + (cameraDistanceVector * 0.5f);

            // Position the panel in the center of the tape measure line
            var panel = tapeMeasurePanel.Q("tape-measure-panel");
            panel.style.top = (Screen.height - halfPoint.y - (panel.resolvedStyle.height / 2)) * ScreenManager.dpiScaler; // Flip y position so 0 is at top
            panel.style.left = (halfPoint.x - (panel.resolvedStyle.width / 2)) * ScreenManager.dpiScaler;

            // Calculate the distance between the two points
            var distance = (current.transform.position - first.transform.position).magnitude;

            // Calculate the feet and inch components of the distance
            var feet = Math.Floor(distance);
            var inches = Math.Round((distance - feet) * 12, 2);

            // Set the label
            var label = panel.Q<Label>("tape-measure-panel-label");
            label.text = $"{feet}' {inches}\"";
        }
    }
}
