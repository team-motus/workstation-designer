using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.ConstructionElements;
using WorkstationDesigner.Rules;

namespace WorkstationDesigner
{
    /// <summary>
    /// Base class for simulation substations.
    /// </summary>
    public abstract class SimSubstation : MonoBehaviour
    {
        public ElementManifest Inventory { get; private set; }
        protected List<RuleBase> AvailableRules, InstantiatedRules;

        protected abstract void CreateAvailableRules();

        public SimSubstation()
        {
            AvailableRules = new List<RuleBase>();
            CreateAvailableRules();
        }

        public void RemoveElements(ConstructionElement element, int quantity)
        {
            Inventory.RemoveElements(element, quantity);
        }

        public void AddElements(ConstructionElement element, int quantity)
        {
            Inventory.AddElements(element, quantity);
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// Get the position of the substation.
        ///
        /// NB: This function should later be replaced with some way of determining where workers should stand (possibly hardcoded on a per-substation basis with this as an abstract function).
        /// </summary>
        /// <returns>The position of the substation</returns>
        public Vector3 GetCoords()
        {
            return this.transform.position;
        }
    }
}
