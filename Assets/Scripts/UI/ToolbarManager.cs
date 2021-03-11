using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WorkstationDesigner.Tools;
using WorkstationDesigner.Util;
using WorkstationDesigner.Workstation;

namespace WorkstationDesigner.UI
{
    public class ToolbarManager : VisualElement
    {
        // Assets
        private static VisualTreeAsset dropdownMenuAsset = null;
        private static VisualTreeAsset dropdownMenuItemAsset = null;

        private VisualElement toolbar;
        private bool mouseOverDropdownButton = false;
        private StyleColor? defaultButtonBackgroundColor = null;
        private StyleColor? defaultDropdownBackgroundColor = null;

        public new class UxmlFactory : UxmlFactory<ToolbarManager, UxmlTraits> { }

        public ToolbarManager()
        {
            // Load assets
            if (dropdownMenuAsset == null)
            {
                dropdownMenuAsset = Resources.Load<VisualTreeAsset>("UI/ToolbarDropdownMenu");
            }
            if (dropdownMenuItemAsset == null)
            {
                dropdownMenuItemAsset = Resources.Load<VisualTreeAsset>("UI/ToolbarDropdownMenuItem");
            }

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
            toolbar = this.Q("Toolbar").Q("toolbar-container");

            // Create toolbar buttons
            CreateToolbarButton("file-button", "File", new List<(string, string, Action)>{
                ( "new-workstation-button", "New", () => WorkstationManager.New() ),
                ( "open-workstation-button", "Open", () => WorkstationManager.PromptOpen() ),
                ( "save-workstation-button", "Save", () => WorkstationManager.Save() ),
                ( "save-as-workstation-button", "Save As...", () => WorkstationManager.PromptSaveAs()),
                ( "exit-button", "Exit", () => AppUtil.Exit() )
            });
            // CreateToolbarButton("edit-button", "Edit");
            CreateToolbarButton("tools-button", "Tools", new List<(string, string, Action)>{
                ( "tape-measure-button", "Tape Measure", () => TapeMeasure.UsingTapeMeasure = !TapeMeasure.UsingTapeMeasure ),
            });
            CreateToolbarButton("vr-button", "VR", new List<(string, string, Action)>{
                ( "enter-vr-button", "Enter VR", () => CameraSwitcher.activateXRRigCamera() )
            });
            // CreateToolbarButton("view-button", "View");
            CreateToolbarButton("help-button", "Help", new List<(string, string, Action)>{
                ( "view-help-online-button", "View Help Online", () => System.Diagnostics.Process.Start("https://github.com/team-motus/workstation-designer") )
            });

            UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        /// <summary>
        /// For a given toolbar button name and dropdown item name under it, mark the item as enabled or disabled
        /// 
        /// Disabled items are grey-out and no longer respond to user input
        /// </summary>
        /// <param name="toolbarButtonName"></param>
        /// <param name="dropdownItemName"></param>
        /// <param name="enabled"></param>
        public static void SetToolbarDropdownItemEnabled(string toolbarButtonName, string dropdownItemName, bool enabled)
        {
            var toolbar = ScreenManager.OverallContainer.Q("Toolbar").Q("toolbar-container");
            if (toolbar == null)
            {
                throw new Exception("Failed to find toolbar");
            }
            var button = toolbar.Q(toolbarButtonName);
            if (button == null)
            {
                throw new Exception("Failed to find toolbar button " + toolbarButtonName);
            }
            var dropdownItem = button.Q(dropdownItemName);
            if (dropdownItem == null)
            {
                throw new Exception("Failed to find dropdown item " + dropdownItem);
            }
            dropdownItem.SetEnabled(enabled);
        }

        /// <summary>
        /// Create a toolbar button with a dropdown menu
        /// </summary>
        /// <param name="elementName">The name of the toolbar VisualElement</param>
        /// <param name="label">The label on the button</param>
        /// <param name="dropdownMenuItems">A list of dropdown menu items</param>
        private void CreateToolbarButton(string elementName, string label, List<(string name, string label, Action clickCallback)> dropdownMenuItems = null)
        {
            var toolbarButtonContainer = dropdownMenuAsset.CloneTree().Q("toolbar-button-container");
            toolbarButtonContainer.name = elementName;
            toolbar.Add(toolbarButtonContainer);

            var toolbarButton = toolbarButtonContainer.Q<Button>("button");
            toolbarButton.text = label;
            var dropdownMenu = toolbarButtonContainer.Q("dropdown-menu");

            if (!defaultButtonBackgroundColor.HasValue)
            {
                // Backup button background color so we can restore it after changing it programatically later
                defaultButtonBackgroundColor = toolbarButtonContainer.resolvedStyle.backgroundColor;
            }
            if (!defaultDropdownBackgroundColor.HasValue)
            {
                // Determine dropdown background color so we can change selected toolbar button's background color to match
                defaultDropdownBackgroundColor = dropdownMenu.resolvedStyle.backgroundColor;
            }

            if (dropdownMenuItems == null || dropdownMenuItems.Count == 0)
            {
                return;
            }

            toolbarButton.RegisterCallback<ClickEvent>(e =>
            {
                SetDropdownVisible(toolbarButton, dropdownMenu, true);
            });
            toolbarButton.RegisterCallback<FocusOutEvent>(e =>
            {
                // Clicking on dropdown buttons also loses focus on the toolbarButton, 
                // so here we check to make sure the mouse isn't over one of those buttons when the focus is lost.
                // If it is over, then we close the dropdown in the dropdown button callback below.
                if (!mouseOverDropdownButton)
                {
                    SetDropdownVisible(toolbarButton, dropdownMenu, false);
                }
            });

            // Setup all dropdown button callbacks
            foreach (var dropdownCallback in dropdownMenuItems)
            {
                CreateDropdownMenuItem(toolbarButtonContainer, dropdownMenu, dropdownCallback.name, dropdownCallback.label, dropdownCallback.clickCallback);
            }
        }

        /// <summary>
        /// Set up a dropdown menu item
        /// </summary>
        /// <param name="toolbarButtonContainer"></param>
        /// <param name="dropdownMenu">The dropdown menu in which the button lies</param>
        /// <param name="dropdownMenuItemName">The name of the button</param>
        /// <param name="dropdownMenuItemLabel"></param>
        /// <param name="clickCallback">The callback to perform when the button is clicked</param>
        private void CreateDropdownMenuItem(VisualElement toolbarButtonContainer, VisualElement dropdownMenu, string dropdownMenuItemName, string dropdownMenuItemLabel, Action clickCallback)
        {
            var dropdownButton = dropdownMenuItemAsset.CloneTree().Q("toolbar-dropdown-menu-item");
            dropdownMenu.Add(dropdownButton);
            dropdownButton.name = dropdownMenuItemName;
            dropdownButton.Q<Button>().text = dropdownMenuItemLabel;

            // var dropdownButton = dropdownMenu.Q(buttonName);
            if (dropdownButton == null)
            {
                throw new Exception($"ToolbarManager: failed to set up toolbar button dropdown callback for button with name \"{dropdownMenuItemName}\"");
            }
            else
            {
                // These callbacks are used to track when the mouse is over a dropdown button
                dropdownButton.RegisterCallback<MouseEnterEvent>(e => mouseOverDropdownButton = true);
                dropdownButton.RegisterCallback<MouseLeaveEvent>(e => mouseOverDropdownButton = false);

                // When the button is clicked, close the dropdown menu then call the provided callback
                dropdownButton.RegisterCallback<ClickEvent>(e =>
                {
                    SetDropdownVisible(toolbarButtonContainer, dropdownMenu, false);
                    clickCallback();
                });
            }
        }

        private void SetDropdownVisible(VisualElement toolbarButtonContainer, VisualElement dropdownMenu, bool visible)
        {
            if (defaultButtonBackgroundColor.HasValue && defaultDropdownBackgroundColor.HasValue)
            {
                // TODO this causes a bug where the on hover styling of the toolbar button itself no longer renders
                // Set button's background color to match that of dropdown menu if it's open
                toolbarButtonContainer.style.backgroundColor = visible ? defaultDropdownBackgroundColor.Value : defaultButtonBackgroundColor.Value;
            }

            dropdownMenu.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
