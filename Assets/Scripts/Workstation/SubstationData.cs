using Newtonsoft.Json;
using System;
using UnityEngine;
using WorkstationDesigner.Substations;

namespace WorkstationDesigner.Workstation
{
    /// <summary>
    /// Represents the data associated with a substation within a workstation
    /// </summary>
    [Serializable]
    public class SubstationData
    {
        /// <summary>
        /// The position/transform data for the substation GameObject
        /// </summary>
        [JsonProperty]
        public TransformData Transform { get; private set; }

        /// <summary>
        /// The substation type itself
        /// </summary>
        [JsonProperty]
        public SubstationBase Substation { get; private set; }

        /// <summary>
        /// Creates a new SubstationData object from a substation GameObject
        /// </summary>
        /// <param name="substationGameObject"></param>
        /// <returns></returns>
        public static SubstationData FromGameObject(GameObject substationGameObject)
        {
            var placedSubstation = substationGameObject.GetComponent<SubstationComponent>();
            if (placedSubstation != null && placedSubstation.Placed)
            {
                return new SubstationData
                {
                    Transform = TransformData.FromTransform(substationGameObject.transform),
                    Substation = placedSubstation.Substation
                };
            }
            throw new Exception("GameObject does not have placed substation component");
        }

        /// <summary>
        /// Create a new GameObject from this SubstationData's transform data and substation type
        /// </summary>
        /// <returns></returns>
        public GameObject RestoreGameObject()
        {
            var substationGameObject = this.Substation.Instantiate();

            this.Transform.SetTransform(substationGameObject.transform);
            
            substationGameObject.AddComponent<SubstationComponent>().Substation = this.Substation;
            
            return substationGameObject;
        }

        public SubstationData()
        {
            Transform = new TransformData();
            Substation = null;
        }
    }
}
