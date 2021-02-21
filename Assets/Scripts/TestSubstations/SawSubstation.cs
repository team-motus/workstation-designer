using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.Elements;
using WorkstationDesigner.Jobs;

namespace WorkstationDesigner.TestSubstations
{
    public class SawSubstation : SimSubstation
    {
        private WoodPlank OutputPlank;
        private SubstationInventory Inventory = new SubstationInventory();
        private bool WoodPlankFilter(Element element)
        {
            return (element is WoodPlank) && (element as WoodPlank).Length == 10;
        }

        private void CompleteSawing(Element plank)
        {
            Inventory.RemoveElements(plank, 1);
            Inventory.AddElements(this.OutputPlank, 2);
            TransportationManager.RegisterAvailability(this, this.OutputPlank, () => this.Inventory.GetQuantity(this.OutputPlank), q => this.Inventory.RemoveElements(this.OutputPlank, q));
        }

        private void AddWoodPlanks(Element plank, int quantity)
        {
            Inventory.AddElements(plank, quantity);
            while (quantity > 0) {
                JobStack.AddJob(new AssemblyJob(() => CompleteSawing(plank), this.GetCoords(), 5, "Cutting wood plank"));
                quantity -= 1;
            }
        }

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
