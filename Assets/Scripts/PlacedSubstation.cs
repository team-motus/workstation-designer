using System;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.InputUtil;
using WorkstationDesigner.UI;

namespace WorkstationDesigner
{
    /// <summary>
    /// A component that has been placed in a workstation.
    /// </summary>
    public class PlacedSubstation : MonoBehaviour
    {
        public SubstationModel Substation { get; set; }
        private const string RIGHT_CLICK_MENU_KEY = "PlacedSubstation";

        private void Awake()
        {
            // Set up right click menu
            if (!RightClickMenuManager.ContainsKey(RIGHT_CLICK_MENU_KEY))
            {
                RightClickMenuManager.Create(RIGHT_CLICK_MENU_KEY, new List<(string, Action<object>)>()
                {
                    ("Pick Up", obj => { 
                        SubstationPlacementManager.Instance.MakePlacementSubstation((GameObject)obj); 
                    }),

                    ("Delete", obj => {
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
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Make sure the raycast hit something
                if(Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        // Open right click menu
                        RightClickMenuManager.Open(RIGHT_CLICK_MENU_KEY, Input.mousePosition, this.gameObject);
                    }
                }
            }
        }
    }
}
