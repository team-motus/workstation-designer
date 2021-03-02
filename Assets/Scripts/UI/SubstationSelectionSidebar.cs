using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using WorkstationDesigner.Substations;

namespace WorkstationDesigner.UI
{
    /// <summary>
    /// A sidebar that contains the list of substations, allowing them to be selected and placed in the workstation.
    /// </summary>
    public class SubstationSelectionSidebar : VisualElement
    {
        private SubstationPlacementManager placementManager;

        public new class UxmlFactory : UxmlFactory<SubstationSelectionSidebar, UxmlTraits> { }

        public SubstationSelectionSidebar()
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

            var listView = new ListView(substationList, itemHeight, makeItem, bindItem);

            listView.selectionType = SelectionType.Multiple;

            listView.onItemsChosen += OnItemsChosen;
            listView.onSelectionChange += OnSelectionChange;

            listView.style.flexGrow = 1.0f; 

            var listContainer = this.Q("SubstationListContainer");
            listContainer.Add(listView);

            placementManager = (SubstationPlacementManager) GameObject.Find("SubstationPlacementManager").GetComponent("SubstationPlacementManager");

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
