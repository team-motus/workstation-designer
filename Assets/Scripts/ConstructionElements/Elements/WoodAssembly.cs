using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.ConstructionElements
{
    public class WoodAssembly : ConstructionElement
    {
        private const string NUM_PLANKS = "num_planks";

        public WoodAssembly() : base("Wood Assembly", new List<string>() { NUM_PLANKS }, 0, true)
        {
        }

        public WoodAssembly(double num_planks) : this()
        {
            SetParameterValue(NUM_PLANKS, num_planks);
        }

        public double GetNumPlanks()
        {
            return GetParameterValue(NUM_PLANKS);
        }
    }
}
