using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace WorkstationDesigner.UI
{
    /// <summary>
    /// Workstation requirements list sidebar
    /// </summary>
    public class WorkstationRequirementsList : SidebarManager.ISidebar
    {
        private static VisualTreeAsset requirementsItemAsset = null;
        private const int ItemHeight = 25; // px

        private ListView listView;

        private List<Requirement> requirements;

        /// <summary>
        /// Represents a workstation requirement -  TODO: this is a placeholder for the backend implementation
        /// </summary>
        public class Requirement
        {
            public string Name { get; private set; }

            public int Outstanding { get; private set; }

            public Requirement(string name, int outstanding)
            {
                this.Name = name;
                this.Outstanding = outstanding;
            }
        }

        public WorkstationRequirementsList()
        {
            // Load assets
            if (requirementsItemAsset == null)
            {
                requirementsItemAsset = Resources.Load<VisualTreeAsset>("UI/WorkstationRequirementsItem");
            }

            requirements = new List<Requirement>();

            // TODO test values
            requirements.Add(new Requirement("Test requirement A", 5));
            requirements.Add(new Requirement("Test requirement B", 2));
            requirements.Add(new Requirement("Test requirement C", 1));

            Func<VisualElement> makeItem = () => requirementsItemAsset.CloneTree();

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                var label = e.Q<Label>("requirements-label");
                label.text = $"{requirements[i].Name} x{requirements[i].Outstanding}";
            };

            listView = new ListView(requirements, ItemHeight, makeItem, bindItem);
            listView.selectionType = SelectionType.Single;

            listView.onItemsChosen += selected =>
            {
                if (selected.First() is Requirement requirement){
                    OnRequirementSelected(requirement);
                }
            };
        }

        private void OnRequirementSelected(Requirement requirement)
        {
            Debug.Log($"TODO Requirement Selected {requirement.Name}");
            SidebarManager.SetSidebar(new SubstationSelectionList(requirement));
        }

        public string GetHeaderText()
        {
            return "Requirements";
        }

        public VisualElement GetBody()
        {
            return listView;
        }
    }
}
