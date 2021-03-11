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

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
