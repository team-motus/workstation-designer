using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.ConstructionElements;
using WorkstationDesigner.Rules;

namespace WorkstationDesigner.Jobs
{
    public class WorkstationSimManager : MonoBehaviour
    {

        private List<ProductionRule> ProductionRules;
        private List<SupplyRule> SupplyRules;
        private List<ExternalInputRule> ExternalInputRules;

        public WorkstationSimManager()
        {
            ResetSimulation();
        }

        public void RegisterProductionRule(ProductionRule productionRule)
        {
            this.ProductionRules.Add(productionRule);
        }

        public void RegisterSupplyRule(SupplyRule supplyRule)
        {
            this.SupplyRules.Add(supplyRule);
        }

        public void RegisterExternalInputRule(ExternalInputRule externalInputRule)
        {
            this.ExternalInputRules.Add(externalInputRule);
        }

        private void CreateTransportationJobs(SimSubstation startSubstation, SimSubstation endSubstation, ConstructionElement element, int quantity, Action fulfilledCallback)
        {
            while (quantity > 0) {
                int transportQuantity = Math.Min(element.MaxCarryable, quantity);
                Action pickupCallback = () =>
                {
                    startSubstation.RemoveElements(element, transportQuantity);
                };
                Action deliveryCallback = () =>
                {
                    endSubstation.AddElements(element, transportQuantity);
                    if (fulfilledCallback != null)
                    {
                        fulfilledCallback();
                    }
                };

                TransportationJob job = new TransportationJob(pickupCallback, deliveryCallback, element, transportQuantity, startSubstation.GetCoords(), endSubstation.GetCoords());
                JobList.AddJob(job);
                quantity -= element.MaxCarryable;
            }
        }

        public void RegisterRequirement(SimSubstation substation, ConstructionElement element, int quantity, Action fulfilledCallback = null)
        {
            foreach(SupplyRule supplyRule in SupplyRules)
            {
                int quantityToSupply = Math.Min(quantity, supplyRule.QuantitySuppliable(element));
                if (quantityToSupply > 0) {
                    CreateTransportationJobs(supplyRule.Substation, substation, element, quantityToSupply, fulfilledCallback);
                }
                quantity -= quantityToSupply;
                if (quantity == 0)
                {
                    return;
                }
            }

            foreach (ProductionRule productionRule in ProductionRules)
            {
                int productionQuantity = productionRule.ProductionQuantity(element);
                if (productionQuantity > 0)
                {
                    // TODO: This may leave some leftover unused outputs at the production substation. May want to make those usable in the future.
                    int productions = (quantity + 1) / productionQuantity; // Round up
                    while (productions > 0)
                    {
                        productionRule.InitiateProduction(() =>
                        {
                            CreateTransportationJobs(productionRule.Substation, substation, element, quantity, fulfilledCallback);
                        });
                        productions -= 1;
                    }

                    return;
                }
            }

            foreach(ExternalInputRule externalInputRule in ExternalInputRules)
            {
                if (externalInputRule.CanSupply(element))
                {
                    // TODO
                }
            }

            throw new UnableToMeetRequirementException();
        }

        private void ResetSimulation()
        {
            this.ProductionRules = new List<ProductionRule>();
            this.SupplyRules = new List<SupplyRule>();
            this.ExternalInputRules = new List<ExternalInputRule>();
            JobList.Clear();
        }

        private void StartSimulation()
        {
            ResetSimulation();

        }

        public void Update()
        {
            if (Input.GetKeyDown("p"))
            {
                StartSimulation();
            }
        }
    }
}
