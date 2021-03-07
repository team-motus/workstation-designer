using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WorkstationDesigner.InputUtil;
using WorkstationDesigner.Substations;
using WorkstationDesigner.UI;
using WorkstationDesigner.Workstation;

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
        private Material DefaultMaterial = null;

        /// <summary>
        /// Set substation placed and change appropriate behavior
        /// </summary>
        /// <param name="placedValue"></param>
        public void SetPlaced(bool placedValue)
        {
            this.gameObject.layer = placedValue ? 0 : 2; // Default or Ignore raycast Layer
            Placed = placedValue;
        }

        /// <summary>
        /// Select a material to use when not highlighted depending on if it's intersecting or not
        /// </summary>
        /// <returns></returns>
        private Material GetUnhighlightedMaterial()
        {
            return IsIntersecting ? IntersectionMaterial : DefaultMaterial;
        }

        public void Awake()
        {
            WorkstationManager.MarkUnsavedChanges();

            // Load/backup materials
            if (DefaultMaterial == null)
            {
                DefaultMaterial = this.GetComponent<Renderer>().sharedMaterial;
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
            this.GetComponent<Renderer>().sharedMaterial = GetUnhighlightedMaterial();

            SetPlaced(true);

            // Set up right click menu
            if (!RightClickMenuManager.ContainsKey(RIGHT_CLICK_MENU_KEY))
            {
                RightClickMenuManager.Create(RIGHT_CLICK_MENU_KEY, new List<(string, Action<object>)>()
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
                        Selected = true;

                        // Set up escape button to deselect this substation
                        Action escAction = () =>
                        {
                            RightClickMenuManager.Close();
                            Selected = false;
                            this.GetComponent<Renderer>().sharedMaterial = GetUnhighlightedMaterial();
                        };
                        EscManager.PushEscAction(escAction);

                        this.GetComponent<Renderer>().sharedMaterial = HighlightedMaterial;

                        // Open right click menu
                        RightClickMenuManager.Open(RIGHT_CLICK_MENU_KEY, Mouse.current.position.ReadValue(), this.gameObject, () =>
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
            Vector3? maybePlacePoint = SubstationPlacementManager.GetPlacementPoint();
            if (maybePlacePoint.HasValue)
            {
                Vector3 placePoint = maybePlacePoint.Value;
                placePoint.y += this.transform.localScale.y / 2;
                this.transform.position = placePoint;

                this.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                this.GetComponent<Renderer>().enabled = false;
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

        private void OnTriggerEnter(Collider collider)
        {
            var intersecting = GetIntersecting(collider);
            if (intersecting != null)
            {
                IntersectionCount++;
                // Update held component's shader
                this.GetComponent<Renderer>().sharedMaterial = GetUnhighlightedMaterial();
                if (!Placed)
                {
                    // Update placed component's shader when there's at least one collision
                    intersecting.GetComponent<Renderer>().sharedMaterial = CheckIntersectionMaterial;
                }
            }
        }

        private void OnTriggerExit(Collider otherCollider)
        {
            if (GetIntersecting(otherCollider) != null)
            {
                IntersectionCount--;
                if (Placed && !IsIntersecting)
                {
                    // Restore placed component's shader to default when all intersections end
                    this.GetComponent<Renderer>().sharedMaterial = DefaultMaterial;
                }
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
    }
}
