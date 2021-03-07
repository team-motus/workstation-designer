using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner
{
    /// <summary>
    /// Handles whether the XR Rig or editor camera is active
    /// </summary>
    public static class CameraSwitcher
    {
        private static GameObject XRRig = GameObject.FindWithTag("XRRig");
        private static GameObject EditorCamera = GameObject.FindWithTag("MainCamera");

        private static bool XRRigActive = false;
        private static bool EditorCameraActive = true;
    
        /// <summary>
        /// Activates the XR rig and deactivates the editor camaera in the scene view
        /// </summary>
        public static void activateXRRigCamera()
        {
            XRRig.SetActive(true);
            EditorCamera.SetActive(false);
            XRRigActive = true;
            EditorCameraActive = false;
        }

        /// <summary>
        /// Activates the editor camera and deactivates the XR rig in the scene view
        /// </summary>
        public static void activateEditorCamera()
        {
            XRRig.SetActive(false);
            EditorCamera.SetActive(true);
            XRRigActive = false;
            EditorCameraActive = true;
        }

        public static bool getXRRigStatus()
        {
            return XRRigActive;
        }

        public static bool getEditorCameraStatus()
        {
            return EditorCameraActive;
        }
    }
}
