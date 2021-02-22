using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorkstationDesigner.Elements;
using WorkstationDesigner.Jobs;

namespace WorkstationDesigner
{
    /// <summary>
    /// Manages transportation and production for all substations.
    ///
    /// Substations register producables with this class if they are able to produce elements when required.
    /// Substations register availailities with this class if they have elements available for pickup.
    /// Substations register requirements with this class if they require elements be delivered to them.s
    ///
    /// The TransportationManager will work to fulfill the registered requirements, either by delivery from a substation that has
    /// the required elements available or by production of the required elements at a capable substation.
    /// </summary>
    public static class TransportationManager
    {
        public delegate int GetQuantityMethod();
        public delegate void RemoveQuantityMethod(int quantityToRemove);

        public delegate void InitiateProductionMethod(int quantityToProduce);

        public delegate bool FilterFunction(Element elementTemplate);
        public delegate void AddElementsMethod(Element element, int quantityToAdd);

        /// <summary>
        /// Base class for the interfaces substations can register with the TransportationManager.
        /// </summary>
        private abstract class Interface
        {
            public SimSubstation Substation;

            protected Interface(SimSubstation substation)
            {
                this.Substation = substation;
            }
        }

        /// <summary>
        /// Class representing the ability to produce elements at a substation.
        /// </summary>
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

        /// <summary>
        /// Class representing the availability of elements at a substation.
        /// </summary>
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

        /// <summary>
        /// Class representing the requirment of elements by a substation.
        /// </summary>
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

        /// <summary>
        /// Registers that a substation can produce an element.
        /// </summary>
        /// <param name="substation">The substation that can produce the element</param>
        /// <param name="elementTemplate">The element that can be produced</param>
        /// <param name="initiateProduction">A callback used to initiate production of the element</param>
        public static void RegisterProducable(SimSubstation substation, Element elementTemplate, InitiateProductionMethod initiateProduction)
        {
            Producable producable = new Producable(substation, elementTemplate, initiateProduction);
            Producables.Add(producable);

            // Check if a requirement already exists that matches this production
            Requirement matchingRequirement = Requirements.Where(requirement => requirement.FilterFunction(elementTemplate)).FirstOrDefault();
            if (matchingRequirement != null)
            {
                // If one does, initiate the production
                initiateProduction(matchingRequirement.Quantity);
                matchingRequirement.BeingProduced = true;
            }
        }

        /// <summary>
        /// Registers that a substation has an element available.
        /// </summary>
        /// <param name="substation">The substation with the element available</param>
        /// <param name="elementTemplate">The element that can be produced</param>
        /// <param name="getQuantity">A callback to get the remaining quantity of the element</param>
        /// <param name="removeQuantity">A callback to remove a quantity of the element (upon pickup)</param>
        public static void RegisterAvailability(SimSubstation substation, Element elementTemplate, GetQuantityMethod getQuantity, RemoveQuantityMethod removeQuantity)
        {
            Availability availability = new Availability(substation, elementTemplate, getQuantity, removeQuantity);
            Availabilities.Add(availability);

            // Check if a requirement already exists that matches this availability
            Requirement matchingRequirement = Requirements.Where(requirement => requirement.FilterFunction(elementTemplate)).FirstOrDefault();
            if (matchingRequirement != null)
            {
                // If one does, create enough TransportationJobs to transport the required quantity
                int remaining = matchingRequirement.Quantity;
                while (remaining > 0)
                {
                    int transportQuantity = Math.Min(elementTemplate.MaxCarryable, remaining);
                    TransportationJob job = new TransportationJob(() => availability.RemoveQuantity(transportQuantity), () => matchingRequirement.AddElements(elementTemplate, transportQuantity), elementTemplate, transportQuantity, substation.GetCoords(), matchingRequirement.Substation.GetCoords());
                    JobStack.AddJob(job);
                    remaining -= elementTemplate.MaxCarryable;
                }
                Requirements.Remove(matchingRequirement);
                // Remove the availability if this used up the remaining stock
                if (availability.GetQuantity() == 0)
                {
                    Availabilities.Remove(availability);
                }
            }
        }

        /// <summary>
        /// Register that a substation requires an element.
        /// </summary>
        /// <param name="substation">The substation that requires the element</param>
        /// <param name="filterFunction">A filter used to determine if an element matches this requirement</param>
        /// <param name="quantity">The quantity of the element required</param>
        /// <param name="addElements">A callback to add the elements to the substation (upon delivery)</param>
        public static void RegisterRequirement(SimSubstation substation, FilterFunction filterFunction, int quantity, AddElementsMethod addElements)
        {
            // Check if a matching availability already exists
            Availability matchingAvailability = Availabilities.Where(availability => filterFunction(availability.ElementTemplate)).FirstOrDefault();
            if (matchingAvailability != null)
            {
                // If one does, create enough TransportationJobs to transport the required quantity
                int remaining = quantity;
                while (remaining > 0)
                {
                    int transportQuantity = Math.Min(matchingAvailability.ElementTemplate.MaxCarryable, remaining);
                    TransportationJob job = new TransportationJob(() => matchingAvailability.RemoveQuantity(transportQuantity), () => addElements(matchingAvailability.ElementTemplate, transportQuantity), matchingAvailability.ElementTemplate, transportQuantity, matchingAvailability.Substation.GetCoords(), substation.GetCoords());
                    JobStack.AddJob(job);
                    remaining -= matchingAvailability.ElementTemplate.MaxCarryable;
                }
                // Don't create a requirement in this case
                return;
            }

            Requirement requirement = new Requirement(substation, filterFunction, quantity, addElements);

            // Check if a matching producable already exists
            Producable matchingProducable = Producables.Where(producable => filterFunction(producable.ElementTemplate)).FirstOrDefault();
            if (matchingProducable != null)
            {
                // If one does, initiate production
                matchingProducable.InitiateProduction(quantity);
                requirement.BeingProduced = true;
            }

            Requirements.Add(requirement);
        }
    }
}
