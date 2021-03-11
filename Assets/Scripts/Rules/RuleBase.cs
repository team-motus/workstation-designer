using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.ConstructionElements;
using WorkstationDesigner.Jobs;

namespace WorkstationDesigner.Rules
{
    public abstract class RuleBase
    {
        public SimSubstation Substation;
        private ElementManifest SubstationInventory;
        protected WorkstationSimManager Manager;
        protected bool Instantiated;

        public RuleBase(SimSubstation substation)
        {
            this.Substation = substation;
            this.Manager = GameObject.Find("WorkstationSimManager").GetComponent<WorkstationSimManager>();
            this.Instantiated = false;
        }

        public abstract void RegisterInterfaces();
    }
}
