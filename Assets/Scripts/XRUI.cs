using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace WorkstationDesigner
{
    public class XRUI : MonoBehaviour
    {
        private GameObject canvas;
        private GameObject rController;
        private Camera xrCam;
        private CameraSwitcher status;

        //private Vector3 yRot;
        //private Vector3 camDistance;

        private ActionBasedController c;

        public int distanceFromVRCam = -3;
        public int height = 2;

        private bool showUI = false;

        void Start()
        {
            //xrCam = GameObject.FindWithTag("MainCamera");
            xrCam = Camera.main;
            canvas = GameObject.Find("Canvas");
            canvas.SetActive(false);

            GameObject temp = GameObject.Find("Camera Handler");
            status = temp.GetComponent<CameraSwitcher>();
        }

        // Update is called once per frame
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

        void updateXRUI()
        {
            canvas.transform.position = targetDistance();
            canvas.transform.eulerAngles = targetRotation();
        }

        Vector3 targetRotation()
        {
            return new Vector3(0, xrCam.transform.eulerAngles.y, 0);
        }

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
