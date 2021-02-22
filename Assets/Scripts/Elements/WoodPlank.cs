using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.Elements
{
    /// <summary>
    /// A wood plank.
    /// </summary>
    public class WoodPlank : Element
    {
        public int Length;

        /// <summary>
        /// Create a wood plank of a certain length.
        /// </summary>
        /// <param name="length">The length of the wood plank in inches</param>
        public WoodPlank(int length) : base(1)
        {
            this.Length = length;
        }

        /// <summary>
        /// Create a text description of the wood plank.
        /// </summary>
        /// <returns>A text description of the wood plank</returns>
        public override string ToString()
        {
            return "Wood plank (" + Length + " inches)";
        }
    }
}
