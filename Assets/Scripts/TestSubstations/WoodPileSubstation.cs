using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.ConstructionElements;
using WorkstationDesigner.ConstructionElements.Elements;
using WorkstationDesigner.Rules;

namespace WorkstationDesigner.TestSubstations
{
    /// <summary>
    /// A test substation representing a pile of wood.
    /// </summary>
    public class WoodPileSubstation : SimSubstation
    {
        private WoodPlank WoodPlank;

        protected override void CreateAvailableRules()
        {
            AvailableRules.Add(new SupplyRule(this, new WoodPlank(), 50));
        }

        /// <summary>
        /// Callback used to get the quantity of wood plank available.
        /// </summary>
        /// <returns>The available quantity of wood planks</returns>
        public int GetWoodQuantity()
        {
            return Inventory.GetQuantity(this.WoodPlank);
        }

        /// <summary>
        /// Callback used to remove wood planks upon pickup.
        /// </summary>
        /// <param name="quantity">The quantity of wood planks to remove</param>
        public void RemoveWood(int quantity)
        {
            Inventory.RemoveElements(this.WoodPlank, quantity);
        }

        // Start is called before the first frame update
        void Start()
        {
            this.WoodPlank = new WoodPlank(10);
            this.Inventory.AddElements(this.WoodPlank, 50);

            TransportationManagerOld.RegisterAvailability(this, this.WoodPlank, GetWoodQuantity, RemoveWood);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
