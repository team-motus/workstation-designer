using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.Workstation
{
    [Serializable]
    public class WorkstationData
    {
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects
        };

        [JsonProperty]
        public List<SubstationData> SubstationList { get; private set; }

        public WorkstationData()
        {
            SubstationList = new List<SubstationData>();
        }

        public static WorkstationData FromGameObject(GameObject workstationObject)
        {
            WorkstationData data = new WorkstationData();
            foreach(Transform childTransform in workstationObject.transform)
            {
                if (childTransform.gameObject.GetComponent<PlacedSubstation>() != null)
                {
                    data.SubstationList.Add(SubstationData.FromGameObject(childTransform.gameObject));
                }
            }

            return data;
        }

        public void PopulateWorkstationObject(GameObject workstationObject)
        {
            foreach (SubstationData substationData in SubstationList)
            {
                var substationObject = substationData.RestoreGameObject();
                substationObject.transform.parent = workstationObject.transform;
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, jsonSerializerSettings);
        }

        public static WorkstationData FromJson(string json)
        {
            return JsonConvert.DeserializeObject<WorkstationData>(json, jsonSerializerSettings);
        }
    }
}
