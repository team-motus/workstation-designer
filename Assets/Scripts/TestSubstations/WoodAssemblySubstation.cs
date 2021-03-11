using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorkstationDesigner.ConstructionElements;
using WorkstationDesigner.ConstructionElements.Elements;
using WorkstationDesigner.Jobs;
using WorkstationDesigner.Rules;

namespace WorkstationDesigner.TestSubstations
{
    /// <summary>
    /// A test substation representing a final wood plank assembly station.
    /// </summary>
    public class WoodAssemblySubstation : SimSubstation
    {
        protected override void CreateAvailableRules()
        {
            ProductionRule assemblyProduction = new ProductionRule(this, new AssemblyJob(GetCoords(), 2, "Putting together wood assembly"));
            assemblyProduction.AddInput(new WoodPlank(), 2);
            assemblyProduction.AddOutput(new WoodAssembly(), 1);
            AvailableRules.Add(assemblyProduction);
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
