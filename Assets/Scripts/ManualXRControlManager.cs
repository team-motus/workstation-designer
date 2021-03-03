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
        CameraSwitcher camSwitch;
        void Start()
        {
            GameObject temp = GameObject.Find("Camera Handler");
            camSwitch = temp.GetComponent<CameraSwitcher>();
        }

        void Update()
        {
            if(Keyboard.current[Key.P].isPressed)
                camSwitch.activateXRRigCamera();

            if(Keyboard.current[Key.O].isPressed)
                camSwitch.activateEditorCamera();
        }
    }
}
