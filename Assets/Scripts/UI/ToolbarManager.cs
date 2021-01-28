using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WorkstationDesigner.Util;

namespace WorkstationDesigner.UI
{
    public class ToolbarManager : VisualElement
    {
        private VisualElement toolbar;
        private static bool mouseOverDropdownButton = false;

        public new class UxmlFactory : UxmlFactory<ToolbarManager, UxmlTraits> { }

        public ToolbarManager()
        {
            RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        void OnGeometryChange(GeometryChangedEvent evt)
        {
            toolbar = this.Q("Toolbar");

            SetupToolbarButton("file-button", new Dictionary<string, EventCallback<ClickEvent>>{
                { "load-workstation-button", e => Debug.Log("TODO load-workstation-button") },
                { "exit-button", e => AppUtil.Exit() } 
            });
            SetupToolbarButton("edit-button");
            SetupToolbarButton("vr-button");
            SetupToolbarButton("view-button");
            SetupToolbarButton("help-button");

            UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        /// <summary>
        /// Setup a toolbar button with a given name and callbacks for its dropdown buttons
        /// </summary>
        /// <param name="name">The name of the toolbar button in the UXML</param>
        /// <param name="dropdownCallbacks">A dictionary of dropdown button names to ClickEvent callbacks</param>
        void SetupToolbarButton(string name, Dictionary<string, EventCallback<ClickEvent>> dropdownCallbacks = null)
        {
            var toolbarButton = toolbar.Q(name);
            var button = toolbarButton.Q("button");
            var dropdown = toolbarButton.Q("dropdown-menu");
            
            toolbarButton.RegisterCallback<FocusInEvent>(e => {
                dropdown.style.display = DisplayStyle.Flex; // Make dropdown visible
            }); 
            toolbarButton.RegisterCallback<FocusOutEvent>(e => {
                // Clicking on dropdown buttons also loses focus on the toolbarButton, 
                // so here we check to make sure the mouse isn't over one of those buttons when the focus is lost.
                // If it is over, then we close the dropdown in the dropdown button callback below.
                if (!mouseOverDropdownButton)
                {
                    dropdown.style.display = DisplayStyle.None; // Make dropdown hidden
                }
            }, TrickleDown.TrickleDown); 
            
            
            if (dropdownCallbacks != null && dropdownCallbacks.Count > 0)
            {
                // Setup all dropdown button callbacks
                foreach (var dropdownCallback in dropdownCallbacks)
                {
                    SetupDropdownCallback(dropdown, dropdownCallback.Key, dropdownCallback.Value);
                }
            }
        }

        /// <summary>
        /// Set up a callback for a dropdown button
        /// </summary>
        /// <param name="dropdownMenu">The dropdown menu in which the button lies</param>
        /// <param name="buttonName">The name of the button</param>
        /// <param name="callback">The callback to perform when the button is clicked</param>
        void SetupDropdownCallback(VisualElement dropdownMenu, string buttonName, EventCallback<ClickEvent> callback)
        {
            var dropdownButton = dropdownMenu.Q(buttonName);
            if (dropdownButton == null)
            {
                throw new System.Exception($"ToolbarManager: failed to set up toolbar button dropdown callback for button with name \"{buttonName}\"");
            }
            else
            {
                // These callbacks are used to track when the mouse is over a dropdown button
                dropdownButton.RegisterCallback<MouseEnterEvent>(e => mouseOverDropdownButton = true);
                dropdownMenu.RegisterCallback<MouseLeaveEvent>(e => mouseOverDropdownButton = false);

                // When the button is clicked, close the dropdown menu then call the provided callback
                dropdownButton.RegisterCallback<ClickEvent>(e =>
                {
                    dropdownMenu.style.display = DisplayStyle.None;
                    callback(e);
                });
            }
        }
    }
}
