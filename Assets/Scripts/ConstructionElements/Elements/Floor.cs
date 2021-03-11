using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.ConstructionElements.Elements
{
    public class Floor : ConstructionElement
    {
        private const string LENGTH = "length";
        private const string WIDTH = "width";

        public Floor() : base("Floor", new List<string>() { LENGTH, WIDTH }, 1, true)
        {
        }

        public Floor(double length, double width) : this()
        {
            SetParameterValue(LENGTH, length);
            SetParameterValue(WIDTH, width);
        }

        public double GetLength()
        {
            return GetParameterValue(LENGTH);
        }

        public double GetWidth()
        {
            return GetParameterValue(WIDTH);
        }
    }
}
