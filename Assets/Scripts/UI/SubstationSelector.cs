using UnityEngine;
using UnityEngine.UIElements;
using WorkstationDesigner.Substations;

namespace WorkstationDesigner.UI
{
    public class SubstationSelector : VisualElement
    {
        private SubstationBase substation;

        public new class UxmlFactory : UxmlFactory<SubstationSelector, UxmlTraits> { }

        public SubstationSelector()
        {
            var selector = Resources.Load<VisualTreeAsset>("UI/SubstationSelector");
            selector.CloneTree(this);
        }

        public void SetSubstation(SubstationBase substation)
        {
            this.substation = substation;
            (this.Q("SubstationName") as Label).text = substation.Name;
        }

        public string GetName()
        {
            return this.substation.Name;
        }
    }
}
