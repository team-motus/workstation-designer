using Newtonsoft.Json;
using System;
using UnityEngine;
using WorkstationDesigner.Substations;

namespace WorkstationDesigner.Workstation
{
    [Serializable]
    public class SubstationData
    {
        [JsonProperty]
        public TransformData Transform { get; private set; }

        [JsonProperty]
        public SubstationBase Substation { get; private set; }

        public static SubstationData FromGameObject(GameObject substationGameObject)
        {
            var placedSubstation = substationGameObject.GetComponent<PlacedSubstation>();
            if (placedSubstation != null)
            {
                return new SubstationData
                {
                    Transform = TransformData.FromTransform(substationGameObject.transform),
                    Substation = placedSubstation.Substation
                };
            }
            throw new Exception("GameObject does not have PlacedSubstation component");
        }

        public GameObject RestoreGameObject()
        {
            var substationGameObject = this.Substation.Instantiate();

            this.Transform.SetTransform(substationGameObject.transform);
            
            substationGameObject.AddComponent<PlacedSubstation>().Substation = this.Substation;
            
            return substationGameObject;
        }

        public SubstationData()
        {
            Transform = new TransformData();
            Substation = null;
        }
    }
}
