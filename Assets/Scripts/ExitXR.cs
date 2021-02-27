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
        private CameraSwitcher CamS;
        private ActionBasedController c;
        void Start()
        {
            c = GetComponent<ActionBasedController>();
        }

        // Update is called once per frame
        void Update()
        {
            c.selectAction.action.performed += Action_preformed; // calls Action_performed() when the select Action is activated
        }

        /// <summary>
        /// Activates the editor camera and deactivates the XR camera when called
        /// </summary>
        private void Action_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            CamS.activateEditorCamera();
        }
    }
}
