using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace WorkstationDesigner.UI
{
    /// <summary>
    /// A sidebar that contains the list of components, allowing them to be selected and placed in the workstation.
    /// </summary>
    public class ComponentSelectionSidebar : VisualElement
    {
        private ComponentPlacementManager placementManager;

        public new class UxmlFactory : UxmlFactory<ComponentSelectionSidebar, UxmlTraits> { }

        public ComponentSelectionSidebar()
        {
            RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        public void OnGeometryChange(GeometryChangedEvent evt)
        {
            // Create some list of data, here simply numbers in interval [1, 1000]
            List<ComponentModel> componentsList = ComponentData.GetInstance().GetComponents();

            // The "makeItem" function will be called as needed
            // when the ListView needs more items to render
            Func<VisualElement> makeItem = () => new ComponentSelector();

            // As the user scrolls through the list, the ListView object
            // will recycle elements created by the "makeItem"
            // and invoke the "bindItem" callback to associate
            // the element with the matching data item (specified as an index in the list)
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                (e as ComponentSelector).SetComponent(componentsList[i]);
            };

            // Provide the list view with an explict height for every row
            // so it can calculate how many items to actually display
            const int itemHeight = 16;

            var listView = new ListView(componentsList, itemHeight, makeItem, bindItem);

            listView.selectionType = SelectionType.Multiple;

            listView.onItemsChosen += OnItemsChosen;
            listView.onSelectionChange += OnSelectionChange;

            listView.style.flexGrow = 1.0f;

            var listContainer = this.Q("ComponentListContainer");
            listContainer.Add(listView);

            placementManager = (ComponentPlacementManager) GameObject.Find("ComponentPlacementManager").GetComponent("ComponentPlacementManager");

            UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        private void OnItemsChosen(IEnumerable<object> objects)
        {
            placementManager.ActivateComponent(objects.First() as ComponentModel);
        }

        private void OnSelectionChange(IEnumerable<object> objects)
        {
        }
    }
}
