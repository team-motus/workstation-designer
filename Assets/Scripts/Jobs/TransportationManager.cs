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

        public delegate bool FilterFunction(Element element);
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
            public Element Element;
            public InitiateProductionMethod InitiateProduction;
            public Producable(SimSubstation substation, Element element, InitiateProductionMethod initiateProduction) : base(substation)
            {
                this.Element = element;
                this.InitiateProduction = initiateProduction;
            }
        }

        /// <summary>
        /// Class representing the availability of elements at a substation.
        /// </summary>
        private class Availability : Interface
        {
            public Element Element;
            public GetQuantityMethod GetQuantity;
            public RemoveQuantityMethod RemoveQuantity;
            
            public Availability(SimSubstation substation, Element element, GetQuantityMethod getQuantity, RemoveQuantityMethod removeQuantity) : base(substation)
            {
                this.Element = element;
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
        /// <param name="element">The element that can be produced</param>
        /// <param name="initiateProduction">A callback used to initiate production of the element</param>
        public static void RegisterProducable(SimSubstation substation, Element element, InitiateProductionMethod initiateProduction)
        {
            Producable producable = new Producable(substation, element, initiateProduction);
            Producables.Add(producable);

            // Check if requirements already exist that matches this production
            int quantityToProduce = 0;
            List<Requirement> matchingRequirements = Requirements.Where(requirement => !requirement.BeingProduced && requirement.FilterFunction(element)).ToList();
            foreach (Requirement matchingRequirement in matchingRequirements)
            {
                // If they do, mark them as being produced and add the quantity required
                matchingRequirement.BeingProduced = true;
                quantityToProduce += matchingRequirement.Quantity;
            }

            // Initiate production if matching requirements were found
            if (quantityToProduce > 0)
            {
                initiateProduction(quantityToProduce);
            }
        }

        /// <summary>
        /// Registers that a substation has an element available.
        /// </summary>
        /// <param name="substation">The substation with the element available</param>
        /// <param name="element">The element that can be produced</param>
        /// <param name="getQuantity">A callback to get the remaining quantity of the element</param>
        /// <param name="removeQuantity">A callback to remove a quantity of the element (upon pickup)</param>
        public static void RegisterAvailability(SimSubstation substation, Element element, GetQuantityMethod getQuantity, RemoveQuantityMethod removeQuantity)
        {
            Availability availability = new Availability(substation, element, getQuantity, removeQuantity);
            Availabilities.Add(availability);

            // Check if requirements already exist that matches this availability
            List<Requirement> matchingRequirements = Requirements.Where(requirement => requirement.FilterFunction(element)).ToList();
            foreach (Requirement matchingRequirement in matchingRequirements)
            {
                // If one does, create enough TransportationJobs to transport the available quantity
                int totalTransportQuantity = Math.Min(getQuantity(), matchingRequirement.Quantity);

                int remaining = totalTransportQuantity;
                while (remaining > 0)
                {
                    int transportQuantity = Math.Min(element.MaxCarryable, remaining);
                    TransportationJob job = new TransportationJob(() => availability.RemoveQuantity(transportQuantity), () => matchingRequirement.AddElements(element, transportQuantity), element, transportQuantity, substation.GetCoords(), matchingRequirement.Substation.GetCoords());
                    JobList.AddJob(job);
                    remaining -= element.MaxCarryable;
                }

                matchingRequirement.Quantity -= totalTransportQuantity;

                // Remove the requirement if this transported the required quantity
                if (matchingRequirement.Quantity == 0)
                {
                    Requirements.Remove(matchingRequirement);
                }

                // Remove the availability if this used up the remaining stock
                if (availability.GetQuantity() == 0)
                {
                    Availabilities.Remove(availability);
                    break;
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
            Requirement requirement = new Requirement(substation, filterFunction, quantity, addElements);

            // Check if matching availabilities already exist
            List<Availability> matchingAvailabilities = Availabilities.Where(availability => filterFunction(availability.Element)).ToList();
            foreach (Availability matchingAvailability in matchingAvailabilities)
            {
                // If one does, create enough TransportationJobs to transport the available quantity
                int totalTransportQuantity = Math.Min(matchingAvailability.GetQuantity(), quantity);

                int remaining = totalTransportQuantity;
                while (remaining > 0)
                {
                    int transportQuantity = Math.Min(matchingAvailability.Element.MaxCarryable, remaining);
                    TransportationJob job = new TransportationJob(() => matchingAvailability.RemoveQuantity(transportQuantity), () => addElements(matchingAvailability.Element, transportQuantity), matchingAvailability.Element, transportQuantity, matchingAvailability.Substation.GetCoords(), substation.GetCoords());
                    JobList.AddJob(job);
                    remaining -= matchingAvailability.Element.MaxCarryable;
                }

                requirement.Quantity -= totalTransportQuantity;

                // Remove the availability if this used up the remaining stock
                if (matchingAvailability.GetQuantity() == 0)
                {
                    Availabilities.Remove(matchingAvailability);
                    break;
                }

                // Stop if this will deliver the required quantity
                if (requirement.Quantity == 0)
                {
                    return;
                }
            }

            // Check if a matching producable already exists
            // Only need to look for one here since we're assuming producables can produce arbitrary quantities of their output
            Producable matchingProducable = Producables.Where(producable => filterFunction(producable.Element)).FirstOrDefault();
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
