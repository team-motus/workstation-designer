using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace WorkstationDesigner.UI
{
    public class ComponentSelector : VisualElement
    {
        private ComponentModel component;

        public new class UxmlFactory : UxmlFactory<ComponentSelector, UxmlTraits> { }

        public ComponentSelector()
        {
            var selector = Resources.Load<VisualTreeAsset>("UI/ComponentSelector");
            selector.CloneTree(this);
        }

        public void SetComponent(ComponentModel component)
        {
            this.component = component;
            (this.Q("ComponentName") as Label).text = component.Name;
        }

        public string GetName()
        {
            return this.component.Name;
        }
    }
}
