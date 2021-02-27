using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner
{
    public class CameraSwitcher : MonoBehaviour
    {
        private GameObject XRRig;
        private GameObject EditorCamera;

        void Start()
        {
            XRRig = GameObject.FindWithTag("XRRig");
            EditorCamera = GameObject.FindWithTag("EditorCamera");
            activateEditorCamera();
        }
    
        public void activateXRRigCamera()
        {
            XRRig.SetActive(true);
            EditorCamera.SetActive(false);
        }

        public void activateEditorCamera()
        {
            XRRig.SetActive(false);
            EditorCamera.SetActive(true);
        }
    }
}
