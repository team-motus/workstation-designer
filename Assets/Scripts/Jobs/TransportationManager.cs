using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorkstationDesigner.Elements;
using WorkstationDesigner.Jobs;

namespace WorkstationDesigner
{
    public static class TransportationManager
    {
        public delegate int GetQuantityMethod();
        public delegate void RemoveQuantityMethod(int quantityToRemove);

        public delegate void InitiateProductionMethod(int quantityToProduce);

        public delegate bool FilterFunction(Element elementTemplate);
        public delegate void AddElementsMethod(Element element, int quantityToAdd);

        private abstract class Interface
        {
            public SimSubstation Substation;

            protected Interface(SimSubstation substation)
            {
                this.Substation = substation;
            }
        }
        private class Producable : Interface
        {
            public Element ElementTemplate;
            public InitiateProductionMethod InitiateProduction;
            public Producable(SimSubstation substation, Element elementTemplate, InitiateProductionMethod initiateProduction) : base(substation)
            {
                this.ElementTemplate = elementTemplate;
                this.InitiateProduction = initiateProduction;
            }
        }

        private class Availability : Interface
        {
            public Element ElementTemplate;
            public GetQuantityMethod GetQuantity;
            public RemoveQuantityMethod RemoveQuantity;
            
            public Availability(SimSubstation substation, Element elementTemplate, GetQuantityMethod getQuantity, RemoveQuantityMethod removeQuantity) : base(substation)
            {
                this.ElementTemplate = elementTemplate;
                this.GetQuantity = getQuantity;
                this.RemoveQuantity = removeQuantity;
            }
        }

        private class Requirement : Interface {
            public int Quantity;
            public FilterFunction FilterFunction;
            public AddElementsMethod AddElements;
            public bool BeingProduced;

            public Requirement(SimSubstation substation, FilterFunction filterFunction, int quantity, AddElementsMethod addElements) : base(substation)
            {
                this.FilterFunction = filterFunction;
                this.Quantity = quantity;
                this.AddElements = addElements;
                this.BeingProduced = false;
            }
        }

        private static readonly List<Producable> Producables = new List<Producable>();
        private static readonly List<Availability> Availabilities = new List<Availability>();
        private static readonly List<Requirement> Requirements = new List<Requirement>();

        public static void RegisterProducable(SimSubstation substation, Element elementTemplate, InitiateProductionMethod initiateProduction)
        {
            Producable producable = new Producable(substation, elementTemplate, initiateProduction);
            Producables.Add(producable);

            Requirement matchingRequirement = Requirements.Where(requirement => requirement.FilterFunction(elementTemplate)).FirstOrDefault();
            if (matchingRequirement != null)
            {
                initiateProduction(matchingRequirement.Quantity);
                matchingRequirement.BeingProduced = true;
            }
        }

        public static void RegisterAvailability(SimSubstation substation, Element elementTemplate, GetQuantityMethod getQuantity, RemoveQuantityMethod removeQuantity)
        {
            Availability availability = new Availability(substation, elementTemplate, getQuantity, removeQuantity);
            Availabilities.Add(availability);

            Requirement matchingRequirement = Requirements.Where(requirement => requirement.FilterFunction(elementTemplate)).FirstOrDefault();
            if (matchingRequirement != null)
            {
                int remaining = matchingRequirement.Quantity;
                while (remaining > 0)
                {
                    int transportQuantity = Math.Min(elementTemplate.MaxCarryable, remaining);
                    TransportationJob job = new TransportationJob(() => availability.RemoveQuantity(transportQuantity), () => matchingRequirement.AddElements(elementTemplate, transportQuantity), elementTemplate, transportQuantity, substation.GetCoords(), matchingRequirement.Substation.GetCoords());
                    JobStack.AddJob(job);
                    remaining -= elementTemplate.MaxCarryable;
                }
                Requirements.Remove(matchingRequirement);
                if (availability.GetQuantity() == 0)
                {
                    Availabilities.Remove(availability);
                }
            }
        }

        public static void RegisterRequirement(SimSubstation substation, FilterFunction filterFunction, int quantity, AddElementsMethod addElements)
        {
            Availability matchingAvailability = Availabilities.Where(availability => filterFunction(availability.ElementTemplate)).FirstOrDefault();
            if (matchingAvailability != null)
            {
                int remaining = quantity;
                while (remaining > 0)
                {
                    int transportQuantity = Math.Min(matchingAvailability.ElementTemplate.MaxCarryable, remaining);
                    TransportationJob job = new TransportationJob(() => matchingAvailability.RemoveQuantity(transportQuantity), () => addElements(matchingAvailability.ElementTemplate, transportQuantity), matchingAvailability.ElementTemplate, transportQuantity, matchingAvailability.Substation.GetCoords(), substation.GetCoords());
                    JobStack.AddJob(job);
                    remaining -= matchingAvailability.ElementTemplate.MaxCarryable;
                }
                return;
            }

            Requirement requirement = new Requirement(substation, filterFunction, quantity, addElements);

            Producable matchingProducable = Producables.Where(producable => filterFunction(producable.ElementTemplate)).FirstOrDefault();
            if (matchingProducable != null)
            {
                matchingProducable.InitiateProduction(quantity);
                requirement.BeingProduced = true;
            }

            Requirements.Add(requirement);
        }
    }
}
