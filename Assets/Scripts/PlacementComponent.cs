using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner
{
    /// <summary>
    /// A temporary version of a workstation component that is displayed when placing the component in the workspace.
    /// </summary>
    public class PlacementComponent : MonoBehaviour
    {
        public ComponentModel Component { get; set; }
        public bool IsIntersecting { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            this.IsIntersecting = false;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3? maybePlacePoint = ComponentPlacementManager.GetPlacementPoint(this.Component);
            if (maybePlacePoint.HasValue)
            {
                Vector3 placePoint = maybePlacePoint.Value;
                placePoint.y += this.transform.localScale.y / 2;
                this.transform.position = placePoint;

                if (!this.IsIntersecting) {
                    this.GetComponent<Renderer>().enabled = true;
                }
                else
                {
                    this.GetComponent<Renderer>().enabled = false;
                }
            }
            else
            {
                this.GetComponent<Renderer>().enabled = false;
            }
		}

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.GetComponent<PlacedComponent>() != null)
            {
                this.IsIntersecting = true;
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.gameObject.GetComponent<PlacedComponent>() != null)
            {
                this.IsIntersecting = false;
            }
        }
    }
}
