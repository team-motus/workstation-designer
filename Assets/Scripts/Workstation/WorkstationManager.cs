using System.IO;
using UnityEngine;
using WorkstationDesigner.UI;

namespace WorkstationDesigner.Workstation
{
    public static class WorkstationManager
    {
        public static string OpenWorkstation { get; private set; } = null;
        public static bool UnsavedChanges { get; private set; } = false;

        private const string DefaultName = "new workstation";
        private const string DefaultPath = "";
        private const string FileExtension = "json";

        public static void MarkUnsavedChanges()
        {
            UnsavedChanges = true;
        }

        private static void CheckUnsavedChanges()
        {
            if (UnsavedChanges)
            {
                Debug.LogWarning("There are unsaved changes!");
            }
        }

        private static void SetOpenWorkstation(string value)
        {
            OpenWorkstation = value;
            UnsavedChanges = false; // Mark changes as saved

            ToolbarManager.SetToolbarDropdownItemEnabled("file-button", "save-workstation-button", OpenWorkstation != null);
        }

        public static void New()
        {
            CheckUnsavedChanges();

            SetOpenWorkstation(null);

            ClearOpenWorkstation();
        }

        public static void PromptOpen()
        {
            var paths = SFB.StandaloneFileBrowser.OpenFilePanel("Load Workstation", DefaultPath, FileExtension, false);

            if (paths.Length > 0)
            {
                Open(paths[0]);
            }
        }

        private static void Open(string filename)
        {
            CheckUnsavedChanges();

            if (File.Exists(filename))
            {
                ClearOpenWorkstation();

                string json = File.ReadAllText(filename);
                WorkstationData workstationData = WorkstationData.FromJson(json);

                workstationData.PopulateWorkstationObject(SubstationPlacementManager.WorkstationParent);

                SetOpenWorkstation(filename);
            }
            else 
            {
                Debug.LogError("Save file not found.");
            }
        }

        public static void PromptSaveAs()
        {
            var path = SFB.StandaloneFileBrowser.SaveFilePanel("Save Workstation", DefaultPath, DefaultName, FileExtension);

            if (!string.IsNullOrEmpty(path))
            {
                Save(path);
            }
            else
            {
                // TODO warn that it did not save
            }
        }

        public static void Save()
        {
            if(OpenWorkstation != null)
            {
                Save(OpenWorkstation);
            }
            else
            {
                throw new System.Exception("Cannot save new workstation. Must use PromptSaveAs instead.");
            }
        }

        private static void Save(string filename)
        {
            WorkstationData workstationData = WorkstationData.FromGameObject(SubstationPlacementManager.WorkstationParent);

            File.WriteAllText(filename, workstationData.ToJson());

            SetOpenWorkstation(filename);
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
