using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.Elements
{
    public class WoodPlank : Element
    {
        public int Length;

        public WoodPlank(int length) : base(1)
        {
            this.Length = length;
        }

        public override string ToString()
        {
            return "Wood plank (" + Length + " inches)";
        }
    }
}
