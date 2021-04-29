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
            // Misc substations
            RegisterSubstation(new ChopSawSubstation("Chop Saw", 2, 6));
            RegisterSubstation(new TableSawSubstation("Table Saw", 3, 3));
            RegisterSubstation(new WorkbenchSubstation("Workbench", 5, 3));
            RegisterSubstation(new BoxSubstation("Box", 2, 2));
            RegisterSubstation(new PalletSubstation("Pallet", 3, 4 ));

            // Pallets of lumber
            var lumberColorVariance = 0.25f;
            RegisterSubstation(new PalletSubstation("Pallet of 2x4s", 3, 4, "Prefabs/MODS/ConstructionElements/TwoByFour",
                constructionElementColorValueVariance: lumberColorVariance));
            RegisterSubstation(new PalletSubstation("Pallet of 2x6s", 3, 4, "Prefabs/MODS/ConstructionElements/TwoBySix",
                constructionElementColorValueVariance: lumberColorVariance));
            RegisterSubstation(new PalletSubstation("Pallet of 2x8s", 3, 4, "Prefabs/MODS/ConstructionElements/TwoByEight",
                constructionElementColorValueVariance: lumberColorVariance));
            RegisterSubstation(new PalletSubstation("Pallet of 2x10s", 3, 4, "Prefabs/MODS/ConstructionElements/TwoByTen",
                constructionElementColorValueVariance: lumberColorVariance));
            RegisterSubstation(new PalletSubstation("Pallet of 2x12s", 3, 4, "Prefabs/MODS/ConstructionElements/TwoByTwelve",
                constructionElementColorValueVariance: lumberColorVariance));

            // Pallets of windows
            RegisterSubstation(new PalletSubstation("Pallet of 2ft x 2ft Windows", 3, 4, "Prefabs/MODS/ConstructionElements/Window2ftx2ft", 
                extendConstructionElementLength: false, constructionElementColorValueVariance: 0));
            RegisterSubstation(new PalletSubstation("Pallet of 2ft x 3ft Windows", 3, 4, "Prefabs/MODS/ConstructionElements/Window_2ftx3ft",
                extendConstructionElementLength: false, constructionElementColorValueVariance: 0));
            RegisterSubstation(new PalletSubstation("Pallet of 3ft x 5ft Windows", 3, 4, "Prefabs/MODS/ConstructionElements/Window_3ftx5ft",
                extendConstructionElementLength: false, constructionElementColorValueVariance: 0));

            // Pallets of drywall sheets
            RegisterSubstation(new PalletSubstation("Pallet of 5/8\" Drywall Sheets", 3, 4, "Prefabs/MODS/ConstructionElements/DrywallSheet_FiveEighthsInch",
                extendConstructionElementLength: false, constructionElementColorValueVariance: 0));
            RegisterSubstation(new PalletSubstation("Pallet of 1/2\" Drywall Sheets", 3, 4, "Prefabs/MODS/ConstructionElements/DrywallSheet_HalfInch",
                extendConstructionElementLength: false, constructionElementColorValueVariance: 0));
            RegisterSubstation(new PalletSubstation("Pallet of 3/8\" Drywall Sheets", 3, 4, "Prefabs/MODS/ConstructionElements/DrywallSheet_ThreeEighthsInch",
                extendConstructionElementLength: false, constructionElementColorValueVariance: 0));
            RegisterSubstation(new PalletSubstation("Pallet of 3/4\" Drywall Sheets", 3, 4, "Prefabs/MODS/ConstructionElements/DrywallSheet_ThreeQuarterInch",
                extendConstructionElementLength: false, constructionElementColorValueVariance: 0));

            // Pallets of particleboard sheets
            RegisterSubstation(new PalletSubstation("Pallet of 5/8\" Particleboard Sheets", 3, 4, "Prefabs/MODS/ConstructionElements/ParticleboardSheet_FiveEighthsInch",
                extendConstructionElementLength: false, constructionElementColorValueVariance: lumberColorVariance));
            RegisterSubstation(new PalletSubstation("Pallet of 1/2\" Particleboard Sheets", 3, 4, "Prefabs/MODS/ConstructionElements/ParticleboardSheet_HalfInch",
               extendConstructionElementLength: false, constructionElementColorValueVariance: lumberColorVariance));
            RegisterSubstation(new PalletSubstation("Pallet of 3/8\" Particleboard Sheets", 3, 4, "Prefabs/MODS/ConstructionElements/ParticleboardSheet_ThreeEighthsInch",
               extendConstructionElementLength: false, constructionElementColorValueVariance: lumberColorVariance));
            RegisterSubstation(new PalletSubstation("Pallet of 3/4\" Particleboard Sheets", 3, 4, "Prefabs/MODS/ConstructionElements/ParticleboardSheet_ThreeQuarterInch",
               extendConstructionElementLength: false, constructionElementColorValueVariance: lumberColorVariance));

            // Pallets of plywood sheets
            RegisterSubstation(new PalletSubstation("Pallet of 5/8\" Plywood Sheets", 3, 4, "Prefabs/MODS/ConstructionElements/PlywoodSheet_FiveEighthsInch",
               extendConstructionElementLength: false, constructionElementColorValueVariance: lumberColorVariance));
            RegisterSubstation(new PalletSubstation("Pallet of 1/2\" Plywood Sheets", 3, 4, "Prefabs/MODS/ConstructionElements/PlywoodSheet_HalfInch",
               extendConstructionElementLength: false, constructionElementColorValueVariance: lumberColorVariance));
            RegisterSubstation(new PalletSubstation("Pallet of 3/8\" Plywood Sheets", 3, 4, "Prefabs/MODS/ConstructionElements/PlywoodSheet_ThreeEighthsInch",
               extendConstructionElementLength: false, constructionElementColorValueVariance: lumberColorVariance));
            RegisterSubstation(new PalletSubstation("Pallet of 3/4\" Plywood Sheets", 3, 4, "Prefabs/MODS/ConstructionElements/PlywoodSheet_ThreeQuarterInch",
               extendConstructionElementLength: false, constructionElementColorValueVariance: lumberColorVariance));
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
