using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.ConstructionElements;
using WorkstationDesigner.ConstructionElements.Elements;
using WorkstationDesigner.Jobs;
using WorkstationDesigner.Rules;

namespace WorkstationDesigner.TestSubstations
{
    /// <summary>
    /// A test substation representing a table saw.
    /// </summary>
    public class SawSubstation : SimSubstation
    {
        protected override void CreateAvailableRules()
        {
            ProductionRule sawProduction = new ProductionRule(this, new AssemblyJob(GetCoords(), 2, "Cutting wood plank"));
            sawProduction.AddInput(new WoodPlank(), 1);
            sawProduction.AddOutput(new WoodPlank(), 2);
            AvailableRules.Add(sawProduction);
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
