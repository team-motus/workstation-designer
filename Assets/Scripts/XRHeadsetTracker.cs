using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace WorkstationDesigner
{
    public class XRHeadsetTracker : MonoBehaviour
    {
        //private GameObject xrCam;
        private GameObject canvas;
        private Camera xrCam;

        private Vector3 yRot;
        private Vector3 camDistance;

        private ActionBasedController c;

        public int distanceFromVRCam = -3;
        public int height = 2;
        void Start()
        {
            //xrCam = GameObject.FindWithTag("MainCamera");
            xrCam = Camera.main;
            canvas = GameObject.Find("Canvas");
            GameObject temp = GameObject.Find("RightHand Controller");
            c = temp.GetComponent<ActionBasedController>();
        }

        // Update is called once per frame
        void Update()
        {
            //check for button press
            
            showXRUI();
        }

        void showXRUI()
        {
            yRot = targetRotation();
            camDistance = targetDistance();
            canvas.transform.position = camDistance;
            canvas.transform.eulerAngles = yRot;
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
