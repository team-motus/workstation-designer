using UnityEngine;
using UnityEngine.UIElements;
using WorkstationDesigner.Substations;

namespace WorkstationDesigner.UI
{
    public class SubstationSelectionItem : VisualElement
    {
        private SubstationBase substation;

        public new class UxmlFactory : UxmlFactory<SubstationSelectionItem, UxmlTraits> { }

        public SubstationSelectionItem()
        {
            var selector = Resources.Load<VisualTreeAsset>("UI/SubstationSelectionItem");
            selector.CloneTree(this);
        }

        public void SetSubstation(SubstationBase substation)
        {
            this.substation = substation;
            (this.Q("substation-name") as Label).text = substation.Name;
        }

        public string GetName()
        {
            return this.substation.Name;
        }
    }
}
