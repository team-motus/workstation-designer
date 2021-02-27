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
    public class SampleWall : SimSubstation
    {
        private const int PLANKS_NEEDED = 8;
        private SubstationInventory Inventory = new SubstationInventory();
		public GameObject[] studList;
		public GameObject[] plateList;

        /// <summary>
		/// Enable/disable individual components of a blueprint based on it's construction progress.
        /// <summary>
		void UpdateBluprintComponents(int objIndex, GameObject[] objList)
		{
			objList[objIndex].GetComponent<MeshRenderer>().enabled = true;
		}
		
        /// <summary>
        /// Filter used to determine whether a particular element can be accepted as input.
        /// </summary>
        /// <param name="element">The element to test</param>
        /// <returns>Whether the element can be accepted</returns>
        private bool WoodPlankFilter(Element element)
        {
            return (element is WoodPlank) && (element as WoodPlank).Length == 10;
        }

        /// <summary>
        /// Callback run upon delivery of wood planks.
        /// </summary>
        /// <param name="plank">The type of plank delivered</param>
        /// <param name="quantity">The number of planks delivered</param>
        private void DeliverWoodPlanks(Element plank, int quantity)
        {
			for(int i = 0; i < quantity; i++){
				int temp = Inventory.GetQuantity(plank)+i;
				JobList.AddJob(new AssemblyJob(() => CompleteAssembly(temp), GetCoords(), 5, "Add plank to assembly"));
			}
			Inventory.AddElements(plank, quantity);
        }

        /// <summary>
        /// Callback run upon the completion of assembly.
        /// </summary>
        private void CompleteAssembly(int plankIndex)
        {
			UpdateBluprintComponents(plankIndex, studList);
            Debug.Log("ASSEMBLY COMPLETE!");
        }

///////////////////////////////////////////////////////////
		
        // Start is called before the first frame update
        void Start()
        {
            TransportationManager.RegisterRequirement(this, WoodPlankFilter, PLANKS_NEEDED, DeliverWoodPlanks);
        }
    }
}
