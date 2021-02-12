using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner
{
    public class ComponentData
    {
        private static ComponentData Instance = null;
        private List<ComponentModel> ComponentList;

        public static ComponentData GetInstance()
        {
            if (ComponentData.Instance == null)
            {
                ComponentData.Instance = new ComponentData();
            }
            return ComponentData.Instance;
        }
        
        private ComponentData()
        {
            this.ComponentList = new List<ComponentModel>();
            this.CreateSampleComponents(); // TODO: Get component data from somewhere else
        }

        private void CreateSampleComponents()
        {
            this.ComponentList.Add(new ComponentModel("Test Component 1", 6, 6));
            this.ComponentList.Add(new ComponentModel("Test Component 2", 6, 12));
            this.ComponentList.Add(new ComponentModel("Test Component 3", 12, 12));
        }

        public List<ComponentModel> GetComponents()
        {
            return this.ComponentList;
        }
    }
}
