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
        /// 
        /// If this held substation is intersecting a placed substation
        /// </summary>
        public bool IsIntersecting { get; private set; }

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

        private void SetIntersecting(bool intersectingValue)
        {
            IsIntersecting = intersectingValue;
            this.GetComponent<Renderer>().sharedMaterial = GetUnhighlightedMaterial();
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
            SetPlaced(true);

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
            this.IsIntersecting = false;
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

                        this.GetComponent<Renderer>().sharedMaterial = HighlightedMaterial;

                        // Open right click menu
                        RightClickMenuManager.Open(RIGHT_CLICK_MENU_KEY, Mouse.current.position.ReadValue(), this.gameObject, () =>
                        {
                            this.GetComponent<Renderer>().sharedMaterial = GetUnhighlightedMaterial();
                            Selected = false;
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
            SetIntersecting(CheckIntersecting(collider));
        }

        private void OnTriggerExit(Collider collider)
        {
            SetIntersecting(!CheckIntersecting(collider));
        }

        /// <summary>
        /// Check if this object is colliding with a placed substation
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        private bool CheckIntersecting(Collider collider)
        {
            if (Placed)
            {
                return false;
            }

            var collidingObject = collider.gameObject.GetComponent<SubstationComponent>();
            var result = collidingObject != null && collidingObject.Placed;
            if (result)
            {
                // Set the other object to also check for intersection -- required for the IntersectionMaterial shader to work
                collidingObject.GetComponent<Renderer>().sharedMaterial = CheckIntersectionMaterial;
            }
            return result;
        }
    }
}
