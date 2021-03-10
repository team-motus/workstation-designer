using UnityEngine;
using UnityEngine.UIElements;

namespace WorkstationDesigner.UI
{
    /// <summary>
    /// Manages the workstation editor sidebar
    /// </summary>
    public class SidebarManager : VisualElement
    {
        /// <summary>
        /// A data representation of a sidebar
        /// </summary>
        public interface ISidebar
        {
            public string GetHeaderText();
            public VisualElement GetBody();
        }

        private Label SidebarHeader;
        private VisualElement SidebarBody;

        private static Background? openIcon = null;
        private static Background? hiddenIcon = null;

        private ISidebar activeSidebar = null;
        private static ISidebar defaulSideBar = new WorkstationRequirementsList();

        public new class UxmlFactory : UxmlFactory<SidebarManager, UxmlTraits> { }

        public SidebarManager()
        {
            if (!openIcon.HasValue)
            {
                openIcon = Background.FromTexture2D(Resources.Load<Texture2D>("UI/images/sidebar-open-icon"));
            }
            if (!hiddenIcon.HasValue)
            {
                hiddenIcon = Background.FromTexture2D(Resources.Load<Texture2D>("UI/images/sidebar-hidden-icon"));
            }

            RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        private void OnGeometryChange(GeometryChangedEvent evt)
        {
            var leftSide = this.Q("left");

            // Find header and body elements to populate later
            SidebarHeader = leftSide.Q<Label>("header-text");
            SidebarBody = leftSide.Q("sidebar-body");

            // Setup the sidebar collapse button
            var sidebarCollapseButton = this.Q("sidebar-collapse-button");
            sidebarCollapseButton.RegisterCallback<MouseDownEvent>(e =>
            {
                // Hide or show sidebar
                leftSide.style.display = (leftSide.style.display != DisplayStyle.None) ? DisplayStyle.None : DisplayStyle.Flex;

                // Change button icon
                sidebarCollapseButton.style.backgroundImage = (leftSide.style.display != DisplayStyle.None) ? openIcon.Value : hiddenIcon.Value;
            });

            UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);

            if (activeSidebar == null)
            {
                SetSidebarImpl(defaulSideBar);
            }
        }

        /// <summary>
        /// Set the active sidebar
        /// </summary>
        /// <param name="sidebar"></param>
        public static void SetSidebar(ISidebar sidebar)
        {
            SidebarManager sidebarManager = ScreenManager.OverallContainer.Q<SidebarManager>();

            sidebarManager.SetSidebarImpl(sidebar);
        }

        /// <summary>
        /// Set the active sidebar
        /// </summary>
        /// <param name="sidebar"></param>
        private void SetSidebarImpl(ISidebar sidebar)
        {
            activeSidebar = sidebar;

            // Set the header text
            SidebarHeader.text = activeSidebar == null ? "" : activeSidebar.GetHeaderText();

            // Remove current sidebar
            for (int i = SidebarBody.childCount - 1; i >= 0; i--)
            {
                SidebarBody.RemoveAt(i);
            }

            // Set sidebar body
            if (activeSidebar != null)
            {
                var newBody = activeSidebar.GetBody();
                newBody.style.flexGrow = 1; // Ensure body fills space
                SidebarBody.Add(newBody);
            }
        }
    }
}
