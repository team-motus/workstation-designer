using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorkstationDesigner.Workstation;

namespace WorkstationDesigner.Util
{
    public static class AppUtil
    {
        public static string MainSceneName = "MainScene";

        public delegate void OnSceneStartDelegate();

        private static OnSceneStartDelegate? OnSceneStartCallback = null;

        private static readonly Func<bool> wantsToQuitCallback = () =>
        {
            if (WorkstationManager.UnsavedChanges)
            {
                WorkstationManager.CheckUnsavedChanges(() =>
                {
                    // Remove callback to prevent infinite recursion
                    Application.wantsToQuit -= wantsToQuitCallback;
                    Application.Quit();
                });
                return false; // Prevent application quit
            }
            else
            {
                return true; // Allow quit to continue
            }
        };

        static AppUtil()
        {
            // Check if there are unsaved changes before quitting the application
            Application.wantsToQuit += wantsToQuitCallback;
        }

        public static void Exit()
        {
            if (Application.isEditor)
            {
                // Simulate exiting
                WorkstationManager.CheckUnsavedChanges(() =>
                {
                    Debug.Log("Exited");
                });
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
