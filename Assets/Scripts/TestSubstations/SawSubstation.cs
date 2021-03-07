using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.ConstructionElements;
using WorkstationDesigner.Jobs;

namespace WorkstationDesigner.TestSubstations
{
    /// <summary>
    /// A test substation representing a table saw.
    /// </summary>
    public class SawSubstation : SimSubstation
    {
        private WoodPlank OutputPlank;
        private SubstationInventory Inventory = new SubstationInventory();

        /// <summary>
        /// Filter used to determine whether a particular element can be accepted as input.
        /// </summary>
        /// <param name="element">The element to test</param>
        /// <returns>Whether the element can be accepted</returns>
        private bool WoodPlankFilter(ConstructionElement element)
        {
            return (element is WoodPlank) && (element as WoodPlank).Length == 10;
        }

        /// <summary>
        /// Callback run when sawing is complete.
        /// </summary>
        /// <param name="plank">The plank that was cut in half</param>
        private void CompleteSawing(ConstructionElement plank)
        {
            Inventory.RemoveElements(plank, 1);
            Inventory.AddElements(this.OutputPlank, 2);
            TransportationManager.RegisterAvailability(this, this.OutputPlank, () => this.Inventory.GetQuantity(this.OutputPlank), q => this.Inventory.RemoveElements(this.OutputPlank, q));
        }

        /// <summary>
        /// Callback run upon delivery of wood planks.
        /// </summary>
        /// <param name="plank">The type of plank delivered</param>
        /// <param name="quantity">The number of planks delivered</param>
        private void AddWoodPlanks(ConstructionElement plank, int quantity)
        {
            Inventory.AddElements(plank, quantity);
            while (quantity > 0) {
                JobList.AddJob(new AssemblyJob(() => CompleteSawing(plank), this.GetCoords(), 5, "Cutting wood plank"));
                quantity -= 1;
            }
        }

        /// <summary>
        /// Callback run to initiate sawing planks in half.
        /// </summary>
        /// <param name="quantity">The number of planks to cut</param>
        private void InitiateSawing(int quantity)
        {
            // TODO: Assuming quantity is divisible by 2
            TransportationManager.RegisterRequirement(this, WoodPlankFilter, quantity / 2, AddWoodPlanks);
        }

        // Start is called before the first frame update
        void Start()
        {
            this.OutputPlank = new WoodPlank(5);
            TransportationManager.RegisterProducable(this, this.OutputPlank, InitiateSawing);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
