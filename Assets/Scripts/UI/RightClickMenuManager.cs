using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace WorkstationDesigner.UI
{
    /// <summary>
    /// Manages right click menus, their creation, opening, and callbacks
    /// </summary>
    public class RightClickMenuManager: MonoBehaviour
    {
        // Assets
        private static VisualTreeAsset rightClickMenuAsset = null;
        private static VisualTreeAsset rightClickMenuItemAsset = null;

        // These values must match those in the UXML/USS files
        private const float VERTICAL_MENU_PADDING = 4; // px
        private const float MENU_ITEM_HEIGHT = 20; // px

        /// <summary>
        /// The current open right click menu
        /// </summary>
        private static VisualElement activeRightClickMenu = null;

        /// <summary>
        /// Dictionary of all saved right click menus
        /// </summary>
        private static Dictionary<string, (object, VisualElement)> rightClickMenus = new Dictionary<string, (object, VisualElement)>();

        private static bool openedThisFrame = false;

        void Awake()
        {
            // Load assets

            rightClickMenuAsset = Resources.Load<VisualTreeAsset>("UI/RightClickMenu");
            if (rightClickMenuAsset == null)
            {
                throw new Exception("rightClickMenuAsset is null");
            }
            rightClickMenuItemAsset = Resources.Load<VisualTreeAsset>("UI/RightClickMenuItem");
            if (rightClickMenuItemAsset == null)
            {
                throw new Exception("rightClickMenuItemAsset is null");
            }
        }

        void LateUpdate()
        {
            // Close the menu if the mouse clicks but it wasn't the click that opened the menu
            if (!openedThisFrame)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    Close();
                }
            }
            openedThisFrame = false;
        }

        /// <summary>
        /// Check if menu key already exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainsKey(string key)
        {
            return rightClickMenus.ContainsKey(key);
        }

        /// <summary>
        /// Create a new menu with a given key and list of tuples of menu items and ballbacks
        /// </summary>
        /// <param name="key"></param>
        /// <param name="buttons"></param>
        public static void Create(string key, List<(string, Action<object>)> buttons)
        {
            if (ContainsKey(key))
            {
                throw new Exception($"Cannot create menu with key \"{key}\" since one already exists");
            }

            var element = rightClickMenuAsset.CloneTree();

            var menu = element.Q("menu-container");
            menu.style.height = MENU_ITEM_HEIGHT * buttons.Count + VERTICAL_MENU_PADDING; // Set height of menu to match number of items

            for (var i = 0; i < buttons.Count; i++)
            {
                var label = buttons[i].Item1;
                var callback = buttons[i].Item2;

                var menuItem = rightClickMenuItemAsset.CloneTree();
                var menuItemLabel = menuItem.Q<Label>("menu-item-label");
                menuItemLabel.text = label;

                menuItem.RegisterCallback<MouseDownEvent>(e =>
                {
                    if (e.button == 0)
                    {
                        Close();
                        callback(rightClickMenus[key].Item1);
                    }
                });
                menu.Add(menuItem);
            }
            rightClickMenus.Add(key, (null, element));
        }

        /// <summary>
        /// Close open right click menu
        /// </summary>
        private static void Close()
        {

            if (activeRightClickMenu != null)
            {
                ScreenManager.OverallContainer.Remove(activeRightClickMenu);
                activeRightClickMenu = null;
            }
        }

        /// <summary>
        /// Open a right click menu with a given key at a given position and with a given context
        /// </summary>
        /// <param name="key">The menu key</param>
        /// <param name="position">The position of the cursor</param>
        /// <param name="context">An object that is passed to the menu item button callbacks</param>
        public static void Open(string key, Vector3 position, object context = null)
        {
            if (!ContainsKey(key))
            {
                throw new Exception($"Menu key \"{key}\" does not exist");
            }

            Close();

            rightClickMenus[key] = (context, rightClickMenus[key].Item2); // Update context
            activeRightClickMenu = rightClickMenus[key].Item2;

            // Place menu element in screen
            ScreenManager.OverallContainer.Add(activeRightClickMenu);


            // Set menu position
            var menu = activeRightClickMenu.Q("menu-container");
            menu.style.top = (Screen.height - position.y) * ScreenManager.dpiScaler; // Flip y position so 0 is at top
            menu.style.left = position.x * ScreenManager.dpiScaler;

            openedThisFrame = true;
        }
    }
}
