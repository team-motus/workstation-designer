using Newtonsoft.Json;
using System;
using UnityEngine;

namespace WorkstationDesigner.Substations
{
    /// <summary>
    /// Represents a substation, its associated data, and means to create a new GameObject representation
    /// </summary>
    public abstract class SubstationBase
    {
        /// <summary>
        /// Name of the substation
        /// </summary>
        [JsonProperty]
        public string Name { get; private set; }

        /// <summary>
        /// Dimensions of the footprint of the substation
        /// </summary>
        [JsonProperty]
        public Tuple<int, int> FootprintDimensions { get; private set; }

        public SubstationBase(string name, int footprintLength1, int footprintLength2)
        {
            this.Name = name;
            this.FootprintDimensions = new Tuple<int, int>(footprintLength1, footprintLength2);
        }

        /// <summary>
        /// Create a new GameObject representation of this substation
        /// </summary>
        /// <returns>A newly instantiated GameObject</returns>
        public abstract GameObject Instantiate();
    }
}
