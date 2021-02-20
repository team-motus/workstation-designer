using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.Elements
{
    public abstract class Element
    {
        public int MaxCarryable;

        public Element(int maxCarryable)
        {
            this.MaxCarryable = maxCarryable;
        }
    }
}
