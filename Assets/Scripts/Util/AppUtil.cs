using UnityEngine;
using UnityEngine.SceneManagement;

namespace WorkstationDesigner.Util
{
    public static class AppUtil
    {
        public static string MainSceneName = "MainScene";

        public delegate void OnSceneStartDelegate();

        private static OnSceneStartDelegate? OnSceneStartCallback = null;

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

        public static void LoadSceneAsync(string sceneName, OnSceneStartDelegate onSceneStart = null)
        {
            OnSceneStartCallback = onSceneStart;
            SceneManager.LoadSceneAsync(sceneName);
        }

        public static void OnSceneStart()
        {
            if(OnSceneStartCallback != null)
            {
                OnSceneStartCallback();
            }
            OnSceneStartCallback = null;
        }
    }
}
