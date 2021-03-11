using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WorkstationDesigner
{
    /// <summary>
    /// Activates the XR Rig if P is pressed or activates the Editor Camera if O is pressed.
    /// </summary>
    public class ManualXRControlManager : MonoBehaviour
    {
        void Start()
        {
            //Initial load of the scene set to the editor camera
            CameraSwitcher.activateEditorCamera();
        }

        void Update()
        {
            if(Keyboard.current[Key.P].isPressed)
                CameraSwitcher.activateXRRigCamera();

            if(Keyboard.current[Key.O].isPressed)
                CameraSwitcher.activateEditorCamera();
        }
    }
}
