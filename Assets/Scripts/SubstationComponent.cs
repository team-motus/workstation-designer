using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WorkstationDesigner.InputUtil;
using WorkstationDesigner.Workstation.Substations;
using WorkstationDesigner.UI;
using WorkstationDesigner.Workstation;
using WorkstationDesigner.Util;

namespace WorkstationDesigner
{
    public class SubstationComponent : MonoBehaviour
    {
        private const float ROTATE_SCALAR = 100;
        private const string RIGHT_CLICK_MENU_KEY = "PlacedSubstation";

        public SubstationBase Substation { get; set; }

        /// <summary>
        /// If this substation has been selected
        /// </summary>
        public bool Selected { get; private set; } = false;

        /// <summary>
        /// If the substation is placed in the workstation or is still being positioned
        /// </summary>
        public bool Placed { get; private set; }

        /// <summary>
        /// If this substation is intersecting any other substations
        /// </summary>
        public bool IsIntersecting => IntersectionCount != 0;

        /// <summary>
        /// Number of other substation intersecting this
        /// </summary>
        public int IntersectionCount { get; private set; } = 0;

        // Materials
        private static Material CheckIntersectionMaterial = null;
        private static Material IntersectionMaterial = null;
        private static Material HighlightedMaterial = null;
        private Dictionary<Renderer, Material[]> DefaultMaterials = null;

        private Vector3? bottomPoint = null;

        /// <summary>
        /// Set substation placed and change appropriate behavior
        /// </summary>
        /// <param name="placedValue"></param>
        public void SetPlaced(bool placedValue)
        {
            foreach (var i in gameObject.GetComponentsInChildren<Transform>()) {
                i.gameObject.layer = placedValue ? 0 : 2; // Default or Ignore raycast Layer
            }
            Placed = placedValue;
        }

        /// <summary>
        /// Update the materials for this substation given its status
        /// </summary>
        /// <param name="forceCheckIntersectionMaterial">Force the material to be set to CheckIntersectionMaterial</param>
        private void UpdateMaterials(bool forceCheckIntersectionMaterial = false)
        {
            MapRenderers(renderer => UpdateMaterials(renderer, forceCheckIntersectionMaterial));
        }

        /// <summary>
        /// Update the materials for a given renderer given the substation status
        /// </summary>
        /// <param name="renderer">The render whose materials will be changed</param>
        /// <param name="forceCheckIntersectionMaterial">Force the material to be set to CheckIntersectionMaterial</param>
        private void UpdateMaterials(Renderer renderer, bool forceCheckIntersectionMaterial)
        {
            var newMaterials = new Material[renderer.materials.Length];
            for (var i = 0; i < newMaterials.Length; i++)
            {
                if (forceCheckIntersectionMaterial)
                {
                    newMaterials[i] = CheckIntersectionMaterial;
                }
                else if (Selected)
                {
                    newMaterials[i] = HighlightedMaterial;
                }
                else if (IsIntersecting)
                {
                    newMaterials[i] = IntersectionMaterial;
                }
                else
                {
                    newMaterials[i] = DefaultMaterials[renderer][i];
                }
            }

            renderer.materials = newMaterials;
        }

        /// <summary>
        /// Set this substation as selected or not
        /// 
        /// This changes the highlighting as well
        /// </summary>
        /// <param name="selected"></param>
        public void SetSelected(bool selected)
        {
            if (selected != Selected)
            {
                if (selected)
                {
                    foreach (var i in SubstationPlacementManager.WorkstationParent.GetComponentsInChildren<SubstationComponent>())
                    {
                        i.SetSelected(false);
                    }
                }

                Selected = selected;
				UpdateMaterials();
            }
        }

        public void Awake()
        {
            WorkstationManager.MarkUnsavedChanges();

            // Load/backup materials
            if (DefaultMaterials == null)
            {
                DefaultMaterials = new Dictionary<Renderer, Material[]>();
                MapRenderers(renderer => DefaultMaterials.Add(renderer, renderer.materials));
            }
            if (CheckIntersectionMaterial == null)
            {
                CheckIntersectionMaterial = Resources.Load<Material>("Materials/CheckIntersectionMaterial");
            }
            if (IntersectionMaterial == null)
            {
                IntersectionMaterial = Resources.Load<Material>("Materials/IntersectionMaterial");
            }
            if (HighlightedMaterial == null)
            {
                HighlightedMaterial = Resources.Load<Material>("Materials/HighlightedMaterial");
            }

            SetPlaced(true);
            UpdateMaterials();

            // Set up right click menu
            if (!RightClickMenuToolkit.ContainsKey(RIGHT_CLICK_MENU_KEY))
            {
                RightClickMenuToolkit.Create(RIGHT_CLICK_MENU_KEY, new List<(string, Action<object>)>()
                {
                    ("Pick Up", obj => {
                        WorkstationManager.MarkUnsavedChanges();
                       SubstationPlacementManager.Instance.PickUpSubstation((GameObject)obj);
                    }),

                    ("Delete", obj => {
                        WorkstationManager.MarkUnsavedChanges();
                        Destroy((GameObject)obj);
                    })
                });
            }
        }

        public void Start()
        {
            IntersectionCount = 0;
        }

        public void Update()
        {
            if (Placed)
            {
                UpdatePlaced();
            }
            else
            {
                UpdateNotPlaced();
            }
        }

        /// <summary>
        /// Check if this substation has been selected if it's been placed
        /// </summary>
        private void UpdatePlaced()
        {
            if (MouseManager.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

                // Make sure the raycast hit something
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        SetSelected(true);

                        // Set up escape button to deselect this substation
                        Action escAction = () =>
                        {
                            RightClickMenuToolkit.Close();
                            SetSelected(false);
                        };
                        EscManager.PushEscAction(escAction);

                        // Open right click menu
                        RightClickMenuToolkit.Open(RIGHT_CLICK_MENU_KEY, Mouse.current.position.ReadValue(), this.gameObject, () =>
                        {
                            EscManager.PopEscAction(escAction);
                            escAction();
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Update this substation's position if it's being held
        /// </summary>
        private void UpdateNotPlaced()
        {
            if (bottomPoint == null)
            {
                bottomPoint = SceneUtil.GetBottomPoint(this.gameObject);
            }
            Vector3? maybePlacePoint = SubstationPlacementManager.GetPlacementPoint();
            if (maybePlacePoint.HasValue)
            {
                Vector3 placePoint = maybePlacePoint.Value;
                if (bottomPoint.HasValue)
                {
                    // Ensure bottom of model is touching the grid
                    placePoint.y = -bottomPoint.Value.y;
                }
                this.transform.position = placePoint;

                SetVisible(true);
            }
            else
            {
                SetVisible(false);
            }

            // Rotation
            if (Keyboard.current[Key.X].isPressed)
            {
                this.transform.Rotate(Vector3.up, ROTATE_SCALAR * Time.deltaTime, Space.World);
            }
            if (Keyboard.current[Key.C].isPressed)
            {
                this.transform.Rotate(Vector3.up, -ROTATE_SCALAR * Time.deltaTime, Space.World);
            }
        }

        private void OnTriggerEnter(Collider otherCollider)
        {
            ProcessTriggerEvent(otherCollider, true);
        }

        private void OnTriggerExit(Collider otherCollider)
        {
            ProcessTriggerEvent(otherCollider, false);
        }

        /// <summary>
        /// Process a trigger event, used to check if two substations are intersecting
        /// </summary>
        /// <param name="otherCollider">The other collider involved in the event</param>
        /// <param name="enter">True if its a trigger enter event, false if it's a triggger exit event</param>
        private void ProcessTriggerEvent(Collider otherCollider, bool enter)
        {
            var intersecting = GetIntersecting(otherCollider);
            if (intersecting != null && !Placed)
            {
                IntersectionCount += enter ? 1 : -1;
                // Update held component's shader
                UpdateMaterials();
                // Update placed component's shader when there's at least one collision
                intersecting.UpdateMaterials(enter);
            }
        }

        /// <summary>
        /// Check if this object is colliding with another substation
        /// </summary>
        /// <param name="otherCollider"></param>
        /// <returns></returns>
        private SubstationComponent GetIntersecting(Collider otherCollider)
        {
            return otherCollider.gameObject.GetComponent<SubstationComponent>();
        }

        /// <summary>
        /// Set the substation as visible or not
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisible(bool visible)
        {
            MapRenderers(renderer => renderer.enabled = visible);
        }

        /// <summary>
        /// Apply an action to all the renderers in this substation GameObject and its children
        /// </summary>
        /// <param name="action"></param>
        private void MapRenderers(Action<Renderer> action)
        {
            foreach (var childRenderer in this.gameObject.GetComponentsInChildren<Renderer>())
            {
                if (childRenderer != null)
                {
                    action(childRenderer);
                }
            }
        }
    }
}
