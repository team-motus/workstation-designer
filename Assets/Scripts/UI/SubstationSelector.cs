using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace WorkstationDesigner.UI
{
    public class SubstationSelector : VisualElement
    {
        private SubstationModel substation;

        public new class UxmlFactory : UxmlFactory<SubstationSelector, UxmlTraits> { }

        public SubstationSelector()
        {
            var selector = Resources.Load<VisualTreeAsset>("UI/SubstationSelector");
            selector.CloneTree(this);
        }

        public void SetSubstation(SubstationModel substation)
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
