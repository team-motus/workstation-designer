using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace WorkstationDesigner
{
    /// <summary>
    /// Sets the behavior for the XR UI.
    /// </summary>
    public class XRUI : MonoBehaviour
    {
        private GameObject canvas;
        private GameObject rController = null;
        private Camera xrCam = null;
        private CameraSwitcher status;

        private ActionBasedController c;

        public int distanceFromVRCam = -3;
        public int height = 2;

        private bool showUI = false;

        void Start()
        {
            canvas = GameObject.Find("Canvas");
            canvas.SetActive(false);

            GameObject temp = GameObject.Find("Camera Handler");
            status = temp.GetComponent<CameraSwitcher>();
        }

        void Update()
        {
            if(status.getXRRigStatus() == true)
            {   
                // Assign controller if it is null
                if(rController == null)
                {
                    rController = GameObject.Find("RightHand Controller");
                    c = rController.GetComponent<ActionBasedController>();
                }

                // XR camear must have MainCamera tag applied to it
                if(xrCam == null)
                    xrCam = Camera.current;

                // Check for correct button press on the right hand controller
                // Update UI and show it
                if(c.activateAction.action.triggered == true && showUI == false)
                {
                    updateXRUI();
                    canvas.SetActive(true);
                    showUI = true;
                }
                // If the UI is active and the same button is pressed again, hide the UI
                else if(c.activateAction.action.triggered == true && showUI == true)
                {
                    canvas.SetActive(false);
                    showUI = false;
                }
            }
        }

        /// <summary>
        /// Updates Y rotation and X and Z position of the XR UI.
        /// </summary>
        void updateXRUI()
        {
            canvas.transform.position = targetDistance();
            canvas.transform.eulerAngles = targetRotation();
        }

        /// <summary>
        /// Returns a Vector3 of the main cameras Y angle with X and Z set to 0.
        /// </summary>
        Vector3 targetRotation()
        {
            return new Vector3(0, xrCam.transform.eulerAngles.y, 0);
        }

        /// <summary>
        /// Returns a Vector3 based on the main cameras view position with a fixed height and distance from the camera.
        /// </summary>
        Vector3 targetDistance()
        {
            Vector3 target;
            Matrix4x4 m = xrCam.cameraToWorldMatrix;
            target = m.MultiplyPoint(new Vector3(0, 0, distanceFromVRCam));
            target.y = height; //fixed height

            return target;
        }
    }
}
