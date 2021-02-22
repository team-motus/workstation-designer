using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorkstationDesigner.Elements;
using WorkstationDesigner.Jobs;

namespace WorkstationDesigner.TestSubstations
{
    /// <summary>
    /// A test substation representing a final wood plank assembly station.
    /// </summary>
    public class WoodAssemblySubstation : SimSubstation
    {
        private const int PLANKS_NEEDED = 2;
        private SubstationInventory Inventory = new SubstationInventory();

        /// <summary>
        /// Filter used to determine whether a particular element can be accepted as input.
        /// </summary>
        /// <param name="element">The element to test</param>
        /// <returns>Whether the element can be accepted</returns>
        private bool WoodPlankFilter(Element element)
        {
            return (element is WoodPlank) && (element as WoodPlank).Length == 5;
        }

        /// <summary>
        /// Callback run upon delivery of wood planks.
        /// </summary>
        /// <param name="plank">The type of plank delivered</param>
        /// <param name="quantity">The number of planks delivered</param>
        private void DeliverWoodPlanks(Element plank, int quantity)
        {
            Inventory.AddElements(plank, quantity);
            if (Inventory.GetQuantity(plank) == PLANKS_NEEDED)
            {
                JobStack.AddJob(new AssemblyJob(CompleteAssembly, GetCoords(), 5, "Add plank to assembly"));
            }
        }

        /// <summary>
        /// Callback run upon the completion of assembly.
        /// </summary>
        private void CompleteAssembly()
        {
            Debug.Log("ASSEMBLY COMPLETE!");
        }

        // Start is called before the first frame update
        void Start()
        {
            TransportationManager.RegisterRequirement(this, WoodPlankFilter, PLANKS_NEEDED, DeliverWoodPlanks);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
