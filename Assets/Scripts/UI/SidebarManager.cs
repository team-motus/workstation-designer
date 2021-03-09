using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WorkstationDesigner.Util;

namespace WorkstationDesigner.UI
{
    public class SidebarManager : VisualElement
    {
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

            SidebarHeader = leftSide.Q<Label>("header-text");
            SidebarBody = leftSide.Q("sidebar-body");

            var sidebarCollapseButton = this.Q("sidebar-collapse-button");

            sidebarCollapseButton.RegisterCallback<MouseDownEvent>(e =>
            {
                leftSide.style.display = (leftSide.style.display != DisplayStyle.None) ? DisplayStyle.None : DisplayStyle.Flex;
                sidebarCollapseButton.style.backgroundImage = (leftSide.style.display != DisplayStyle.None) ? openIcon.Value : hiddenIcon.Value;
            });

            UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);

            if (activeSidebar == null)
            {
                SetSidebarImpl(new WorkstationRequirementsList());
            }
        }

        public static void SetSidebar(ISidebar sidebar)
        {
            SidebarManager sidebarManager = ScreenManager.OverallContainer.Q<SidebarManager>();

            sidebarManager.SetSidebarImpl(sidebar);
        }

        private void SetSidebarImpl(ISidebar sidebar)
        {
            activeSidebar = sidebar;

            SidebarHeader.text = activeSidebar == null ? "" : activeSidebar.GetHeaderText();
            for (int i = SidebarBody.childCount - 1; i >= 0; i--)
            {
                SidebarBody.RemoveAt(i);
            }
            if (activeSidebar != null)
            {
                var newBody = activeSidebar.GetBody();
                newBody.style.flexGrow = 1;
                SidebarBody.Add(newBody);
            }
        }
    }
}
