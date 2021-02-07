using UnityEngine;

namespace WorkstationDesigner.Workstation
{
    public static class WorkstationManager
    {
        public static void PromptLoadWorkstation()
        {
            var paths = SFB.StandaloneFileBrowser.OpenFilePanel("Load Workstation", "", "", false);
            if (paths.Length > 0)
            {
                Debug.Log("TODO Load workstation");
            }
        }
    }
}
