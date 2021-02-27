using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace WorkstationDesigner
{
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
            c.selectAction.action.performed += Action_preformed;
        }

        private void Action_preformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            CamS.activateEditorCamera();
        }
    }
}
