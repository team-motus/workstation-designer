using System.IO;
using UnityEngine;

namespace WorkstationDesigner.Workstation
{
    public static class WorkstationManager
    {
        private static string openWorkstation = null;

        public static void New()
        {
            // TODO check if there are unsaved changes first

            openWorkstation = null;

            ClearOpenWorkstation();
        }

        public static void PromptOpen()
        {
            var paths = SFB.StandaloneFileBrowser.OpenFilePanel("Load Workstation", "", "", false);

            if (paths.Length > 0)
            {
                Open(paths[0]);
            }
        }

        private static void Open(string filename)
        {

            if (File.Exists(filename))
            {
                ClearOpenWorkstation();

                string json = File.ReadAllText(filename);
                WorkstationData workstationData = WorkstationData.FromJson(json);

                workstationData.PopulateWorkstationObject(SubstationPlacementManager.WorkstationParent);

                openWorkstation = filename;
            }
            else 
            {
                Debug.LogError("Save file not found.");
            }
        }

        public static void PromptSaveAs()
        {
            var paths = SFB.StandaloneFileBrowser.OpenFilePanel("Save Workstation", "", "", false);

            if (paths.Length > 0)
            {
                Save(paths[0]);
            }
            else
            {
                // TODO warn that it did not save
            }
        }

        public static void Save()
        {
            if(openWorkstation != null)
            {
                Save(openWorkstation);
            }
            else
            {
                PromptSaveAs(); // TODO disable Save button in UI if this is the case
            }
        }

        private static void Save(string filename)
        {
            WorkstationData workstationData = WorkstationData.FromGameObject(SubstationPlacementManager.WorkstationParent);

            File.WriteAllText(filename, workstationData.ToJson());
        }

        public static void ClearOpenWorkstation()
        {
            foreach (Transform childTransform in SubstationPlacementManager.WorkstationParent.transform)
            {
                GameObject.Destroy(childTransform.gameObject);
            }
        }
    }
}
