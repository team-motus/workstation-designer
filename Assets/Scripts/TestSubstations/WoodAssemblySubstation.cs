using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorkstationDesigner.Elements;
using WorkstationDesigner.Jobs;

namespace WorkstationDesigner.TestSubstations
{
    public class WoodAssemblySubstation : SimSubstation
    {
        private const int PLANKS_NEEDED = 2;
        private SubstationInventory Inventory = new SubstationInventory();

        private bool WoodPlankFilter(Element element)
        {
            return (element is WoodPlank) && (element as WoodPlank).Length == 5;
        }

        private void DeliverWoodPlanks(Element element, int quantity)
        {
            Inventory.AddElements(element, quantity);
            if (Inventory.GetQuantity(element) == PLANKS_NEEDED)
            {
                JobStack.AddJob(new AssemblyJob(CompleteAssembly, GetCoords(), 10));
            }
        }

        private void CompleteAssembly()
        {
            Debug.Log("ASSEMBLY COMPLETE!");
        }

        // Start is called before the first frame update
        void Start()
        {
            TransportationManager.RegisterRequirement(this, WoodPlankFilter, PLANKS_NEEDED, DeliverWoodPlanks); // TODO: Can just pass inventory directly (and just pass a callback)?
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
