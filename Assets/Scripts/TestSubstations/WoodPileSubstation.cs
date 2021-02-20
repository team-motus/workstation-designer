using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.Elements;

namespace WorkstationDesigner.TestSubstations
{
    public class WoodPileSubstation : SimSubstation
    {
        private WoodPlank WoodPlank;
        private SubstationInventory Inventory = new SubstationInventory();

        public int GetWoodQuantity()
        {
            return Inventory.GetQuantity(this.WoodPlank);
        }

        public void RemoveWood(int quantity)
        {
            Inventory.RemoveElements(this.WoodPlank, quantity);
        }

        // Start is called before the first frame update
        void Start()
        {
            this.WoodPlank = new WoodPlank(10);
            this.Inventory.AddElements(this.WoodPlank, 50);

            TransportationManager.RegisterAvailability(this, this.WoodPlank, GetWoodQuantity, RemoveWood);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
