using UnityEngine;

namespace WorkstationDesigner.Util
{
    public static class AppUtil
    {
        public static void Exit()
        {
            if (Application.isEditor)
            {
                Debug.Log("Exited");
            }
            else
            {
                Application.Quit();
            }
        }
    }
}
