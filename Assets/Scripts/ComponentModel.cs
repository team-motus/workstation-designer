using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner
{
    /// <summary>
    /// Stores the data associated with a single workstation component.
    /// </summary>
    public class ComponentModel
    {
        public string Name { get; private set; }
        public Tuple<int, int> FootprintDimensions { get; private set; }

        public ComponentModel(string name, int footprintLength1, int footprintLength2)
        {
            this.Name = name;
            this.FootprintDimensions = new Tuple<int, int>(footprintLength1, footprintLength2);
        }
    }
}
