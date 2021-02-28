using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace WorkstationDesigner
{

    /// <summary>
    /// Exits XR Camera when specified controller action is performed
    /// </summary>
    public class ExitXR : MonoBehaviour
    {
        private CameraSwitcher camS;
        private ActionBasedController c;
        void Start()
        {
            GameObject temp = GameObject.FindWithTag("CameraHandler");
            camS = temp.GetComponent<CameraSwitcher>();
            c = GetComponent<ActionBasedController>();
        }

        // Checks to see if the trigger has been pressed every frame
        void Update()
        {
            // Temporary solution for exiting XR camera state
            // Future implementations will use an XR UI to exit XR
            if(c.activateAction.action.triggered == true)
                camS.activateEditorCamera(); // Switch to Editor camera
        }
    }
}
