using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner
{
    /// <summary>
    /// Manages loading and providing access to database of components.
    /// </summary>
    public class SubstationData
    {
        private static SubstationData Instance = null;
        private List<SubstationModel> SubstationList;

        public static SubstationData GetInstance()
        {
            if (SubstationData.Instance == null)
            {
                SubstationData.Instance = new SubstationData();
            }
            return SubstationData.Instance;
        }
        
        private SubstationData()
        {
            this.SubstationList = new List<SubstationModel>();
            this.CreateSampleSubstations(); // TODO: Get substation data from somewhere else
        }

        private void CreateSampleSubstations()
        {
            this.SubstationList.Add(new SubstationModel("Test Substation 1", 6, 6));
            this.SubstationList.Add(new SubstationModel("Test Substation 2", 6, 12));
            this.SubstationList.Add(new SubstationModel("Test Substation 3", 12, 12));
        }

        public List<SubstationModel> GetSubstations()
        {
            return this.SubstationList;
        }
    }
}
