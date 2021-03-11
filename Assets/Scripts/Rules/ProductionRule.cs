using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.ConstructionElements;
using WorkstationDesigner.Jobs;

namespace WorkstationDesigner.Rules
{
    public class ProductionRule : RuleBase
    {
        public delegate bool ExtraPrerequisitesMetCallback();

        private AssemblyJob Job;
        private ElementManifest Inputs, Outputs;
        private ExtraPrerequisitesMetCallback PrerequisitesMet;

        public ProductionRule(SimSubstation substation, AssemblyJob job) : base(substation)
        {
            this.Job = job;
            this.Inputs = new ElementManifest();
            this.Outputs = new ElementManifest();
            this.PrerequisitesMet = null;
        }

        public void AddInput(ConstructionElement input, int quantity)
        {
            this.Inputs.AddElements(input, quantity);
        }

        public void AddOutput(ConstructionElement output, int quantity)
        {
            this.Outputs.AddElements(output, quantity);
        }

        public void SetExtraPrerequisitesCallback(ExtraPrerequisitesMetCallback callback)
        {
            this.PrerequisitesMet = callback;
        }

        private bool RequirementsFulfilled()
        {
            if (!this.Instantiated)
            {
                throw new UnsetParameterException();
            }

            if (this.PrerequisitesMet != null && !this.PrerequisitesMet())
            {
                return false;
            }

            return this.Inputs.Subset(Substation.Inventory);
        }

        public int ProductionQuantity(ConstructionElement element)
        {
            return Outputs.GetQuantity(element);
        }

        public override void RegisterInterfaces()
        {
            Manager.RegisterProductionRule(this);
        }

        public void RegisterRequirements()
        {
            Inputs.ForEach((e, q) =>
            {
                Manager.RegisterRequirement(Substation, e, q, () =>
                {
                    if (RequirementsFulfilled())
                    {
                        JobList.AddJob(Job);
                    }
                });
            });
        }

        public void InitiateProduction(Action completionCallback)
        {
            Job.CompletionCallback = completionCallback;
            RegisterRequirements();
        }
    }
}
