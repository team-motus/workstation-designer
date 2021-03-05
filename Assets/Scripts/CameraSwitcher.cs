using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner
{
    /// <summary>
    /// Handles whether the XR Rig or editor camera is active
    /// </summary>
    public class CameraSwitcher : MonoBehaviour
    {
        private GameObject XRRig;
        private GameObject EditorCamera;

        private bool XRRigActive = false;
        private bool EditorCameraActive = true;

        void Start()
        {
            XRRig = GameObject.FindWithTag("XRRig");
            EditorCamera = GameObject.FindWithTag("MainCamera");
            activateEditorCamera();
        }
    
        /// <summary>
        /// Activates the XR rig and deactivates the editor camaera in the scene view
        /// </summary>
        public void activateXRRigCamera()
        {
            XRRig.SetActive(true);
            EditorCamera.SetActive(false);
            XRRigActive = true;
            EditorCameraActive = false;
        }

        /// <summary>
        /// Activates the editor camera and deactivates the XR rig in the scene view
        /// </summary>
        public void activateEditorCamera()
        {
            XRRig.SetActive(false);
            EditorCamera.SetActive(true);
            XRRigActive = false;
            EditorCameraActive = true;
        }

        public bool getXRRigStatus()
        {
            return XRRigActive;
        }

        public bool getEditorCameraStatus()
        {
            return EditorCameraActive;
        }
    }
}
