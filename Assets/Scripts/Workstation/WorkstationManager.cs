using UnityEngine;

namespace WorkstationDesigner.Workstation
{
    public static class WorkstationManager
    {
        public static void PromptLoadWorkstation()
        {
            var paths = SFB.StandaloneFileBrowser.OpenFilePanel("Load Workstation", "", "", false);

            Debug.Log("TODO Load workstation");

            if (paths.Length > 0)
            {
                // TODO
            }
        }
    }
}
