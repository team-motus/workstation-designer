using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.ConstructionElements.Elements
{
    /// <summary>
    /// A wood plank.
    /// </summary>
    public class WoodPlank : ConstructionElement
    {
        private const string LENGTH = "length";

        public WoodPlank() : base("Wood Plank", new List<string>() { LENGTH }, 1, false)
        {
        }

        /// <summary>
        /// Create a wood plank of a certain length.
        /// </summary>
        /// <param name="length">The length of the wood plank in inches</param>
        public WoodPlank(double length) : this()
        {
            SetParameterValue(LENGTH, length);
        }

        public double GetLength()
        {
            return GetParameterValue(LENGTH);
        }
    }
}
