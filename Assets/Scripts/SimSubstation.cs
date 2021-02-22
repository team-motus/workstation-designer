using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner
{
    /// <summary>
    /// Base class for simulation substations.
    /// </summary>
    public abstract class SimSubstation : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// Get the position of the substation.
        ///
        /// NB: This function should later be replaced with some way of determining where workers should stand (possibly hardcoded on a per-substation basis with this as an abstract function).
        /// </summary>
        /// <returns>The position of the substation</returns>
        public Vector3 GetCoords()
        {
            return this.transform.position;
        }
    }
}
