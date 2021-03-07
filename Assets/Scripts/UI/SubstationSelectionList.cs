using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WorkstationDesigner.Substations;

namespace WorkstationDesigner.UI
{
    /// <summary>
    /// A list of substations, allowing them to be selected and placed in the workstation.
    /// </summary>
    public class SubstationSelectionList : VisualElement
    {
        private SubstationPlacementManager placementManager;

        private VisualElement leftSide;
        private VisualElement SubstationListContainer;
        private ListView listView;

        private static Background openIcon = Background.FromTexture2D(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UI/images/sidebar-open-icon.png"));
        private static Background hiddenIcon = Background.FromTexture2D(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UI/images/sidebar-hidden-icon.png"));

        public new class UxmlFactory : UxmlFactory<SubstationSelectionList, UxmlTraits> { }

        public SubstationSelectionList()
        {
            RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        public void OnGeometryChange(GeometryChangedEvent evt)
        {
            // Create some list of data, here simply numbers in interval [1, 1000]
            List<SubstationBase> substationList = SubstationManager.GetInstance().GetSubstations();

            // The "makeItem" function will be called as needed
            // when the ListView needs more items to render
            Func<VisualElement> makeItem = () => new SubstationSelector();

            // As the user scrolls through the list, the ListView object
            // will recycle elements created by the "makeItem"
            // and invoke the "bindItem" callback to associate
            // the element with the matching data item (specified as an index in the list)
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                (e as SubstationSelector).SetSubstation(substationList[i]);
            };

            // Provide the list view with an explict height for every row
            // so it can calculate how many items to actually display
            const int itemHeight = 16;

            listView = new ListView(substationList, itemHeight, makeItem, bindItem);

            listView.selectionType = SelectionType.Multiple;

            listView.onItemsChosen += OnItemsChosen;
            listView.onSelectionChange += OnSelectionChange;

            listView.style.flexGrow = 1.0f; 

            SubstationListContainer = this.Q("SubstationListContainer");
            SubstationListContainer.Add(listView);

            placementManager = (SubstationPlacementManager) GameObject.Find("SubstationPlacementManager").GetComponent("SubstationPlacementManager");

            leftSide = this.Q("left");

            var sidebarCollapseButton = this.Q("sidebar-collapse-button");

            sidebarCollapseButton.RegisterCallback<MouseDownEvent>(e =>
            {
                leftSide.style.display = (leftSide.style.display != DisplayStyle.None) ? DisplayStyle.None: DisplayStyle.Flex;
                sidebarCollapseButton.style.backgroundImage = (leftSide.style.display != DisplayStyle.None) ? openIcon: hiddenIcon;
            });

            UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        private void OnItemsChosen(IEnumerable<object> objects)
        {
            placementManager.ActivateSubstation(objects.First() as SubstationBase);
        }

        private void OnSelectionChange(IEnumerable<object> objects)
        {

        }
    }
}
