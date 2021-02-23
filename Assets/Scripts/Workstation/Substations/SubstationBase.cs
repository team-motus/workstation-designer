using Newtonsoft.Json;
using System;
using UnityEngine;

namespace WorkstationDesigner.Substations
{
    public abstract class SubstationBase
    {
        // [JsonProperty]
        // public string Type => this.GetType().FullName;

        [JsonProperty]
        public string Name { get; private set; }

        [JsonProperty]
        public Tuple<int, int> FootprintDimensions { get; private set; }

        public SubstationBase(string name, int footprintLength1, int footprintLength2)
        {
            this.Name = name;
            this.FootprintDimensions = new Tuple<int, int>(footprintLength1, footprintLength2);
        }

        public abstract GameObject Instantiate();
    }
}
