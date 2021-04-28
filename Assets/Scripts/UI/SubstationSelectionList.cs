using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using WorkstationDesigner.Workstation.Substations;

namespace WorkstationDesigner.UI
{
    public class SubstationSelectionList : SidebarManager.ISidebar
    {
        private SubstationPlacementManager placementManager;

        private ListView listView;

        private List<SubstationBase> substationList;

        public SubstationSelectionList()
        {
            substationList = SubstationManager.GetInstance().GetSubstations();

            // The "makeItem" function will be called as needed
            // when the ListView needs more items to render
            Func<VisualElement> makeItem = () => new SubstationSelectionItem();

            // As the user scrolls through the list, the ListView object
            // will recycle elements created by the "makeItem"
            // and invoke the "bindItem" callback to associate
            // the element with the matching data item (specified as an index in the list)
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                (e as SubstationSelectionItem).SetSubstation(substationList[i]);
            };

            // Provide the list view with an explict height for every row
            // so it can calculate how many items to actually display
            const int itemHeight = 25; // px

            listView = new ListView(substationList, itemHeight, makeItem, bindItem);

            listView.selectionType = SelectionType.Single;

            listView.onItemsChosen += OnItemsChosen;
            listView.onSelectionChange += OnSelectionChange;

            placementManager = GameObject.Find("SubstationPlacementManager").GetComponent<SubstationPlacementManager>();
        }

        public string GetHeaderText()
        {
            return "Substations";
        }

        public VisualElement GetBody()
        {
            return listView;
        }

        private void OnItemsChosen(IEnumerable<object> objects)
        {
            var subtation = objects.First() as SubstationBase;
            placementManager.CreateSubstation(subtation, () => { });
        }

        private void OnSelectionChange(IEnumerable<object> objects) { }
    }
}
