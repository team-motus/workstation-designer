using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.Workstation.Substations
{
    /// <summary>
    /// Manages loading and providing access to database of components.
    /// </summary>
    public class SubstationManager
    {
        private static SubstationManager Instance = null;
        private List<SubstationBase> SubstationList;

        public static SubstationManager GetInstance()
        {
            if (SubstationManager.Instance == null)
            {
                SubstationManager.Instance = new SubstationManager();
            }
            return SubstationManager.Instance;
        }

        private SubstationManager()
        {
            this.SubstationList = new List<SubstationBase>();
            this.CreateSampleSubstations(); // TODO: Get substation data from somewhere else
        }

        private void CreateSampleSubstations()
        {
            RegisterSubstation(new CubeSubstation("Test Substation 1", 6, 6));
            RegisterSubstation(new CubeSubstation("Test Substation 2", 6, 12));
            RegisterSubstation(new CubeSubstation("Test Substation 3", 12, 12));
            RegisterSubstation(new ChopSawSubstation("Chop Saw", 2, 6));
            RegisterSubstation(new TableSawSubstation("Table Saw", 3, 3));
            RegisterSubstation(new WorkbenchSubstation("Workbench", 5, 3));
        }

        public void RegisterSubstation(SubstationBase substation)
        {

            this.SubstationList.Add(substation);
        }

        public List<SubstationBase> GetSubstations()
        {
            return this.SubstationList;
        }
    }
}
