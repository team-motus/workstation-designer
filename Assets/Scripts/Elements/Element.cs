using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.Elements
{
    /// <summary>
    /// Base class for an element - a transportable, requirable object that can be used in assembly.
    /// </summary>
    public abstract class Element
    {
        public int MaxCarryable;

        /// <summary>
        /// Base constructor for elements.
        /// </summary>
        /// <param name="maxCarryable">The maximum quantity of this element that can be carried at once by a worker</param>
        public Element(int maxCarryable)
        {
            this.MaxCarryable = maxCarryable;
        }
    }
}
