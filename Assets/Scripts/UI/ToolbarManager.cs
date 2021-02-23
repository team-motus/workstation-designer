using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using WorkstationDesigner.Util;
using WorkstationDesigner.Workstation;

namespace WorkstationDesigner.UI
{
    public class ToolbarManager : VisualElement
    {
        private VisualElement toolbar;
        private bool mouseOverDropdownButton = false;
        private StyleColor? defaultButtonBackgroundColor = null;
        private StyleColor? defaultDropdownBackgroundColor = null;

        public new class UxmlFactory : UxmlFactory<ToolbarManager, UxmlTraits> { }

        public ToolbarManager()
        {
            RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        /// <summary>
        /// This function sets up the UI after the after the GeometryChangedEvent, which occurs after layout calculations result in changes
        /// 
        /// In example code by Unity Technologies, this is where they set up user interfaces for custom visual elements.
        /// It sets things up once the layout calculations are complete, and then it unregisters itself as a callback because we won't 
        /// need to set things up again
        /// </summary>
        /// <param name="evt"></param>
        private void OnGeometryChange(GeometryChangedEvent evt)
        {
            toolbar = this.Q("Toolbar");

            SetupToolbarButton("file-button", new Dictionary<string, Action>{
                { "new-workstation-button", () => WorkstationManager.New() },
                { "open-workstation-button", () => WorkstationManager.PromptOpen() },
                { "save-workstation-button", () => WorkstationManager.Save() },
                { "save-as-workstation-button", () => WorkstationManager.PromptSaveAs() },
                { "exit-button", () => AppUtil.Exit() } 
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
        private void SetupToolbarButton(string name, Dictionary<string, Action> dropdownCallbacks = null)
        {
            var toolbarButtonParent = toolbar.Q(name);
            var toolbarButton = toolbarButtonParent.Q("button");
            var dropdownMenu = toolbarButtonParent.Q("dropdown-menu");

            if (!defaultButtonBackgroundColor.HasValue)
            {
                // Backup button background color so we can restore it after changing it programatically later
                defaultButtonBackgroundColor = toolbarButton.resolvedStyle.backgroundColor;
            }
            if (!defaultDropdownBackgroundColor.HasValue)
            {
                // Determine dropdown background color so we can change selected toolbar button's background color to match
                defaultDropdownBackgroundColor = dropdownMenu.resolvedStyle.backgroundColor;
            }

            toolbarButton.RegisterCallback<ClickEvent>(e => {
                SetDropdownVisible(toolbarButton, dropdownMenu, true);
            }); 
            toolbarButton.RegisterCallback<FocusOutEvent>(e => {
                // Clicking on dropdown buttons also loses focus on the toolbarButton, 
                // so here we check to make sure the mouse isn't over one of those buttons when the focus is lost.
                // If it is over, then we close the dropdown in the dropdown button callback below.
                if (!mouseOverDropdownButton)
                {
                    SetDropdownVisible(toolbarButton, dropdownMenu, false);
                }
            }); 
            
            
            if (dropdownCallbacks != null && dropdownCallbacks.Count > 0)
            {
                // Setup all dropdown button callbacks
                foreach (var dropdownCallback in dropdownCallbacks)
                {
                    SetupDropdownCallback(toolbarButton, dropdownMenu, dropdownCallback.Key, dropdownCallback.Value);
                }
            }
        }

        /// <summary>
        /// Set up a callback for a dropdown button
        /// </summary>
        /// <param name="dropdownMenu">The dropdown menu in which the button lies</param>
        /// <param name="buttonName">The name of the button</param>
        /// <param name="callback">The callback to perform when the button is clicked</param>
        private void SetupDropdownCallback(VisualElement toolbarButton, VisualElement dropdownMenu, string buttonName, Action callback)
        {
            var dropdownButton = dropdownMenu.Q(buttonName);
            if (dropdownButton == null)
            {
                throw new Exception($"ToolbarManager: failed to set up toolbar button dropdown callback for button with name \"{buttonName}\"");
            }
            else
            {
                // These callbacks are used to track when the mouse is over a dropdown button
                dropdownButton.RegisterCallback<MouseEnterEvent>(e => mouseOverDropdownButton = true);
                dropdownButton.RegisterCallback<MouseLeaveEvent>(e => mouseOverDropdownButton = false);

                // When the button is clicked, close the dropdown menu then call the provided callback
                dropdownButton.RegisterCallback<ClickEvent>(e =>
                {
                    SetDropdownVisible(toolbarButton, dropdownMenu, false);
                    callback();
                });
            }
        }

        private void SetDropdownVisible(VisualElement button, VisualElement dropdownMenu, bool visible)
        {
            if(defaultButtonBackgroundColor.HasValue && defaultDropdownBackgroundColor.HasValue)
            {
                // Set button's background color to match that of dropdown menu if it's open
                button.style.backgroundColor = visible ? defaultDropdownBackgroundColor.Value : defaultButtonBackgroundColor.Value;
            }

            dropdownMenu.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
