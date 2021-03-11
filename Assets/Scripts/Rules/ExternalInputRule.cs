using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.ConstructionElements;

namespace WorkstationDesigner.Rules
{
    public class ExternalInputRule : RuleBase
    {
        private ElementManifest Elements;

        private Action GetInputs;

        public ExternalInputRule(SimSubstation substation, Action getInputsCallback) : base(substation)
        {
            GetInputs = getInputsCallback;
        }

        public bool CanSupply(ConstructionElement element)
        {
            return Elements.GetQuantity(element) > 0;
        }

        public override void RegisterInterfaces()
        {
            Manager.RegisterExternalInputRule(this);
        }
    }
}
