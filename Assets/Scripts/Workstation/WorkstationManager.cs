using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using WorkstationDesigner.UI;
using WorkstationDesigner.Util;

namespace WorkstationDesigner.Workstation
{
    public static class WorkstationManager
    {
        /// <summary>
        /// The name and path of the currently open workstation
        /// 
        /// If the open workstation is new (has not been saved), then this value is null
        /// </summary>
        public static string OpenWorkstationFullFilename { get; private set; } = null;

        /// <summary>
        /// If there are changes to the open workstation that have not been saved
        /// </summary>
        public static bool UnsavedChanges { get; private set; } = false;

        private const string DefaultName = "new workstation";
        private const string DefaultPath = "";
        private const string FileExtension = "json";

        private const string SAVE_DIALOG_KEY = "SaveDialog";
        private const string ERROR_DIALOG_KEY = "ErrorDialog";

        private const string SaveDialogBodyPath = "UI/SaveDialogBody";
        private const string LoadErrorDialogBodyPath = "UI/LoadErrorDialogBody";

        private static Action next = null;

        static WorkstationManager()
        {
            ResourceLoader.Load<VisualTreeAsset>(SaveDialogBodyPath);
            ResourceLoader.Load<VisualTreeAsset>(LoadErrorDialogBodyPath);
        }

        /// <summary>
        /// Indicate that the open workstation has changed and needs to be saved
        /// </summary>
        public static void MarkUnsavedChanges()
        {
            UnsavedChanges = true;
        }

        /// <summary>
        /// Check if there are unsaved changes and warn the user
        /// </summary>
        public static void CheckUnsavedChanges(Action nextAction)
        {
            if (UnsavedChanges)
            {
                if (next != null)
                {
                    throw new Exception("Cannot overwrite next action callback, please wait for dialog to close");
                }
                next = nextAction;

                // Set up save dialog
                if (!DialogManager.ContainsKey(SAVE_DIALOG_KEY))
                {
                    VisualTreeAsset bodyAsset = ResourceLoader.Get<VisualTreeAsset>(SaveDialogBodyPath);
                    VisualElement body = bodyAsset.CloneTree();
                    DialogManager.Create(SAVE_DIALOG_KEY, body, new List<(string, Action<object>)>()
                    {
                        ("Cancel", obj => {}),
                        ("No", obj => {
                            next();
                            next = null;
                        }),
                        ("Yes", obj => {
                            PromptSaveAs();
                            next();
                            next = null;
                        })
                    });
                }

                DialogManager.Open(SAVE_DIALOG_KEY);
            }
            else
            {
                nextAction();
            }
        }

        private static void CreateErrorDialog()
        {
            if (!DialogManager.ContainsKey(ERROR_DIALOG_KEY))
            {
                VisualTreeAsset bodyAsset = ResourceLoader.Get<VisualTreeAsset>(LoadErrorDialogBodyPath);
                VisualElement body = bodyAsset.CloneTree();

                DialogManager.Create(ERROR_DIALOG_KEY, body, new List<(string, Action<object>)>()
                {
                    ("Okay", obj => {})
                });
            }
        }

        /// <summary>
        /// Set the value of OpenWorkstation
        /// 
        /// This also greys out the Save button if it's a new workstation, which then forces the user to use the Save As button
        /// </summary>
        /// <param name="value"></param>
        private static void SetOpenWorkstation(string value)
        {
            OpenWorkstationFullFilename = value;
            UnsavedChanges = false; // Mark changes as saved

            ToolbarManager.SetToolbarDropdownItemEnabled("file-button", "save-workstation-button", OpenWorkstationFullFilename != null);
        }

        /// <summary>
        /// Create a new, empty workstation (closing any open workstation)
        /// </summary>
        public static void New()
        {
            CheckUnsavedChanges(() =>
            {
                CloseOpenWorkstation();
            });
        }

        /// <summary>
        /// Prompt the user to open a saved workstation file
        /// </summary>
        public static void PromptOpen()
        {
            var paths = SFB.StandaloneFileBrowser.OpenFilePanel("Load Workstation", DefaultPath, FileExtension, false);

            if (paths.Length > 0)
            {
                Open(paths[0]);
            }
        }

        /// <summary>
        /// Open a workstation file with a given filename and path
        /// </summary>
        /// <param name="fullFilename"></param>
        private static void Open(string fullFilename)
        {
            CheckUnsavedChanges(() =>
            {
                if (File.Exists(fullFilename))
                {
                    CloseOpenWorkstation();

                    try
                    {
                        string json = File.ReadAllText(fullFilename);
                        WorkstationData workstationData = WorkstationData.FromJson(json);

                        workstationData.PopulateWorkstationObject(SubstationPlacementManager.WorkstationParent);

                        SetOpenWorkstation(fullFilename);
                    }
                    catch (Exception e)
                    {
                        CreateErrorDialog();

                        Debug.LogError(e);

                        DialogManager.Open(ERROR_DIALOG_KEY, customizeDialog: dialogRootElement =>
                        {
                            Label errorDescription = dialogRootElement.Q<Label>("error-description");
                            string filename = fullFilename.Replace("\\", "/");
                            errorDescription.text = $"Failed to load file \"{filename}\"";

                            Label errorDetails = dialogRootElement.Q<Label>("error-details");
                            errorDetails.text = $"Details: {e.Message}";
                        });
                    }
                }
                else
                {
                    Debug.LogError("Save file not found.");
                }
            });
        }

        /// <summary>
        /// Prompt the user to save the workstation with a new file name
        /// </summary>
        public static void PromptSaveAs()
        {
            var path = SFB.StandaloneFileBrowser.SaveFilePanel("Save Workstation", DefaultPath, DefaultName, FileExtension);

            if (!string.IsNullOrEmpty(path))
            {
                Save(path);
            }
        }

        /// <summary>
        /// Save the open workstation to the currently open workstation file
        /// </summary>
        public static void Save()
        {
            if (OpenWorkstationFullFilename != null)
            {
                Save(OpenWorkstationFullFilename);
            }
            else
            {
                throw new System.Exception("Cannot save new workstation. Must use PromptSaveAs instead.");
            }
        }

        /// <summary>
        /// Save the open workstation to a file with a given name and path
        /// </summary>
        /// <param name="fullFilename"></param>
        private static void Save(string fullFilename)
        {
            WorkstationData workstationData = WorkstationData.FromGameObject(SubstationPlacementManager.WorkstationParent);

            File.WriteAllText(fullFilename, workstationData.ToJson());

            SetOpenWorkstation(fullFilename);
        }

        /// <summary>
        /// Close the open workstation
        /// </summary>
        public static void CloseOpenWorkstation()
        {
            // Destroy all children of workstation parent GameObject
            foreach (Transform childTransform in SubstationPlacementManager.WorkstationParent.transform)
            {
                GameObject.Destroy(childTransform.gameObject);
            }

            SetOpenWorkstation(null);
        }
    }
}
