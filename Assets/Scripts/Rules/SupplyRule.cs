using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.ConstructionElements;

namespace WorkstationDesigner.Rules
{
    public class SupplyRule : RuleBase
    {
        private ElementManifest Elements;

        public SupplyRule(SimSubstation substation, ConstructionElement element, int quantity) : base(substation) { }

        public int QuantitySuppliable(ConstructionElement element)
        {
            return Elements.GetQuantity(element);
        }

        public override void RegisterInterfaces()
        {
            Manager.RegisterSupplyRule(this);
        }
    }
}
