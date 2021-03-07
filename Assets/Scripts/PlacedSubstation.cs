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
    /// <summary>
    /// A component that has been placed in a workstation.
    /// </summary>
    public class PlacedSubstation : MonoBehaviour
    {
        public SubstationBase Substation { get; set; }
        private const string RIGHT_CLICK_MENU_KEY = "PlacedSubstation";

        private static Material IntersectionMaterial = null;

        public void Awake()
        {
            WorkstationManager.MarkUnsavedChanges();

            this.gameObject.layer = 0; // Default
            if (IntersectionMaterial == null)
            {
                IntersectionMaterial = Resources.Load<Material>("Materials/IntersectionMaterial");
            }
            this.GetComponent<Renderer>().sharedMaterial = IntersectionMaterial;

            // Set up right click menu
            if (!RightClickMenuManager.ContainsKey(RIGHT_CLICK_MENU_KEY))
            {
                RightClickMenuManager.Create(RIGHT_CLICK_MENU_KEY, new List<(string, Action<object>)>()
                {
                    ("Pick Up", obj => {
                        WorkstationManager.MarkUnsavedChanges();
                        SubstationPlacementManager.Instance.MakePlacementSubstation((GameObject)obj); 
                    }),

                    ("Delete", obj => {
                        WorkstationManager.MarkUnsavedChanges();
                        Destroy((GameObject)obj);
                    })
                });
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (MouseManager.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

                // Make sure the raycast hit something
                if(Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        // Open right click menu
                        RightClickMenuManager.Open(RIGHT_CLICK_MENU_KEY, Mouse.current.position.ReadValue(), this.gameObject);
                    }
                }
            }
        }
    }
}
