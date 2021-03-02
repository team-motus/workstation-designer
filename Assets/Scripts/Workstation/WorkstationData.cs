using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.Workstation
{
    /// <summary>
    /// Represents all of the data associated with a workstation
    /// </summary>
    [Serializable]
    public class WorkstationData
    {
        /// <summary>
        /// JSON serializer settings used to save and restore this data from disk
        /// </summary>
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects // Handle polymorphism
        };

        // TODO add name

        /// <summary>
        /// A list of all substations within this workstation
        /// </summary>
        [JsonProperty]
        public List<SubstationData> SubstationList { get; private set; }

        public WorkstationData()
        {
            SubstationList = new List<SubstationData>();
        }

        /// <summary>
        /// Create a WorkstationData object from a workstation GameObject
        /// </summary>
        /// <param name="workstationObject"></param>
        /// <returns></returns>
        public static WorkstationData FromGameObject(GameObject workstationObject)
        {
            WorkstationData data = new WorkstationData();
            foreach(Transform childTransform in workstationObject.transform)
            {
                // Add each child as a substation if the child is a PlacedSubstation
                if (childTransform.gameObject.GetComponent<PlacedSubstation>() != null)
                {
                    data.SubstationList.Add(SubstationData.FromGameObject(childTransform.gameObject));
                }
            }

            return data;
        }

        /// <summary>
        /// Setup a workstation GameObject using the data held by this object
        /// 
        /// Essentially creates all the substation GameObjects and adds them as children
        /// </summary>
        /// <param name="workstationObject"></param>
        public void PopulateWorkstationObject(GameObject workstationObject)
        {
            foreach (SubstationData substationData in SubstationList)
            {
                var substationObject = substationData.RestoreGameObject();
                substationObject.transform.parent = workstationObject.transform;
            }
        }

        /// <summary>
        /// Serialize this object to JSON
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, jsonSerializerSettings);
        }

        /// <summary>
        /// Create a new WorkstationData object from JSON
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static WorkstationData FromJson(string json)
        {
            return JsonConvert.DeserializeObject<WorkstationData>(json, jsonSerializerSettings);
        }
    }
}
